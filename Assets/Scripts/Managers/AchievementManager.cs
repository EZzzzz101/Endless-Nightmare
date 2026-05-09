using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ExcelDataReader.Log;
using UnityEngine;

//任务状态
public enum AchievementStatus
{
    NotAccepted,    // 未接取
    InProgress,     // 进行中
    Completable,    // 可领取
    Claimed,        // 已领取
}

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    //所有成就的配置数据
    [Header("成就配置")]
    public List<AchievementData> allAchievements = new List<AchievementData>();

   // 三个字典：进度 / 是否接取 / 是否领取
    private Dictionary<string, int> _progress = new Dictionary<string, int>();
    private Dictionary<string, bool> _accepted = new Dictionary<string, bool>();
    private Dictionary<string, bool> _claimed = new Dictionary<string, bool>();
    //通知ui刷新的事件
    public Action OnProgressUpdated;
    void Awake()
    {
        Instance=this;
    }


    void OnEnable()
    {
          //初始化所有成就为0
        foreach(var ach in allAchievements)
        {
            _progress[ach.achievementID] = 0;
            _claimed[ach.achievementID] = false;
            _accepted[ach.achievementID] = false;
        }
        InventoryModel.Instance.OnSlotChanged += OnBagChanged;
         // 测试
    // _accepted["kill_10_any"] = true;
    // Debug.Log($"测试接取 kill_10_any, 字典里有 {_accepted.Count} 个成就");
    //    if (EnemyManager.Instance != null)
    // EnemyManager.Instance.StartSpawning();
    }
    void OnDestroy()
    {
        InventoryModel.Instance.OnSlotChanged -= OnBagChanged;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            foreach (var ach in allAchievements)
            {
                Debug.Log($"[任务] {ach.achievementID} | 状态={GetStatus(ach.achievementID)} | 进度={GetProgress(ach.achievementID)}/{ach.targetCount}");
            }
        }
    }



    // ========== 各系统调这个来报告进度 ==========

    /// <summary> 对话里接任务 </summary>
    public void AcceptAchievement(string achievementID)
    {
        if (_accepted.ContainsKey(achievementID))
        {
            _accepted[achievementID] = true;
            Debug.Log($"接取成就: {achievementID}");
            OnProgressUpdated?.Invoke();
        }
    }

    /// <summary>
    /// 收集到物品时调用
    /// </summary>
   public void ReportItemCollected(string itemname, int count)
{
    foreach (var ach in allAchievements)
    {
        if (!_accepted.ContainsKey(ach.achievementID) || !_accepted[ach.achievementID]) continue;
        if (_claimed.ContainsKey(ach.achievementID) && _claimed[ach.achievementID]) continue;
        if (ach.type != AchievementType.CollectItem) continue;
        if (ach.targetItemName != itemname) continue;

        _progress[ach.achievementID] += count;
        OnProgressUpdated?.Invoke();
    }
}


    /// <summary>
    /// 杀怪时调用
    /// </summary>
   public void ReportEnemyKilled(string enemyTag, int count)
    {
        foreach (var ach in allAchievements)
        {
            if (!_accepted.ContainsKey(ach.achievementID) || !_accepted[ach.achievementID]) continue;
            if (_claimed.ContainsKey(ach.achievementID) && _claimed[ach.achievementID]) continue;
            if (ach.type != AchievementType.KillEnemy) continue;

            bool match = string.IsNullOrEmpty(ach.targetItemName)
                    || ach.targetItemName == enemyTag;
            if (!match) continue;

            _progress[ach.achievementID] += count;
        }
        OnProgressUpdated?.Invoke();
    }


    /// <summary>
    /// 升级时调用  
    /// </summary>
    public void ReportLevelUp(int newLevel)
    {
        foreach (var ach in allAchievements)
        {
            if (!_accepted[ach.achievementID]) continue;
            if (_claimed[ach.achievementID]) continue;
            if (ach.type != AchievementType.ReachLevel) continue;

            _progress[ach.achievementID] = newLevel;
        }
        OnProgressUpdated?.Invoke();
    }

    // ========== 查询接口（UI 用） ==========
    /// <summary>
    /// 获取某个成就的当前进度
    /// </summary>
    public int GetProgress(string achievementID)
    {
        return _progress.ContainsKey(achievementID) ? _progress[achievementID] : 0;
    }

    /// <summary>
    /// 获取某个成就的当前状态
    /// </summary>
    
    //读取字典返回枚举状态
    public AchievementStatus GetStatus(string achievementID)
    {
        if (_claimed.ContainsKey(achievementID) && _claimed[achievementID])
            return AchievementStatus.Claimed;

        if (!_accepted.ContainsKey(achievementID) || !_accepted[achievementID])
            return AchievementStatus.NotAccepted;

        var ach = allAchievements.Find(a => a.achievementID == achievementID);
        if (ach != null && _progress[achievementID] >= ach.targetCount)
            return AchievementStatus.Completable;

        return AchievementStatus.InProgress;
    }


    /// <summary>
    /// 领取奖励
    /// </summary>
    public bool ClaimReward(string achievementID)
    {
        if (GetStatus(achievementID) != AchievementStatus.Completable) return false;

        var ach = allAchievements.Find(a => a.achievementID == achievementID);
        ScoreManager.Instance.AddScore(ach.rewardScore);
        _claimed[achievementID] = true;


        // 扣除背包物品
         InventoryModel.Instance.RemoveItemByName(ach.targetItemName, ach.targetCount);

        Debug.Log($"成就 [{ach.title}] 已领取，获得 {ach.rewardScore} 分");
        OnProgressUpdated?.Invoke();

        if (AllQuestsClaimed())
        {
            var npc = FindObjectOfType<NPCVisibility>();
            if (npc != null) npc.gameObject.SetActive(false);
        }
        return true;
    }

    /// <summary>
    /// 背包任何格子变化 → 重新统计所有成就进度
    /// </summary>
    void OnBagChanged(int slotIndex, ItemData itemData, int count)
    {
        foreach (var ach in allAchievements)
        {
            if (ach.type != AchievementType.CollectItem) continue;   // ← 加这行
            if (!_accepted[ach.achievementID]) continue;
            if (_claimed[ach.achievementID]) continue;

            int totalInBag = InventoryModel.Instance.GetTotalCount(ach.targetItemName);
            _progress[ach.achievementID] = Mathf.Min(totalInBag, ach.targetCount);
        }
        OnProgressUpdated?.Invoke();
    }


    /// <summary> 存档用：导出所有成就的接取和领取状态 </summary>
    public List<AchievementSaveData> ExportAchievements()
    {
        List<AchievementSaveData> result = new List<AchievementSaveData>();
        foreach (var ach in allAchievements)
        {
            result.Add(new AchievementSaveData
            {
                achievementID = ach.achievementID,
                isAccepted = _accepted.ContainsKey(ach.achievementID) && _accepted[ach.achievementID],
                isClaimed = _claimed.ContainsKey(ach.achievementID) && _claimed[ach.achievementID],
            });
        }
        return result;
    }

    /// <summary> 读档用：恢复接取和领取状态，然后重算进度 </summary>
    public void ImportAchievements(List<AchievementSaveData> list)
    {
        if (list == null) return;

        foreach (var data in list)
        {
            if (_accepted.ContainsKey(data.achievementID))
                _accepted[data.achievementID] = data.isAccepted;
            if (_claimed.ContainsKey(data.achievementID))
                _claimed[data.achievementID] = data.isClaimed;
        }

        // 根据背包重新算所有进度
        RefreshAllProgress();
        OnProgressUpdated?.Invoke();
    }

    /// <summary> 从背包重读所有成就进度 </summary>
    void RefreshAllProgress()
    {
        foreach (var ach in allAchievements)
        {
            if (!_accepted[ach.achievementID]) continue;
            int total = InventoryModel.Instance.GetTotalCount(ach.targetItemName);
            _progress[ach.achievementID] = Mathf.Min(total, ach.targetCount);
        }
    }

    //检测有没有全部领取
    public bool AllQuestsClaimed()
    {
        foreach (var ach in allAchievements)
        {
            var s = GetStatus(ach.achievementID);
            if (s != AchievementStatus.Claimed)
                return false;
        }
        return true;
    }
}
