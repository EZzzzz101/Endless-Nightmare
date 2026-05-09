using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance{get;private set;}

    // 存档文件路径
    private string GetSavePath(string playerId) => Path.Combine(Application.persistentDataPath, $"save_{playerId}.json");

    private string _currentPlayerId;

    void Awake()
    {
        Instance=this;
    }

    /// <summary>
    /// 检查这个 ID 是否已被注册
    /// </summary>
    public bool PlayerExists(string playerId)
    {
        return File.Exists(GetSavePath(playerId));
    }

    /// <summary>
    /// 从各系统收集数据 → 序列化为 JSON → 写入文件
    /// </summary>
    public void Save(string playerId)
    {
        _currentPlayerId = playerId;
        SaveData data = new SaveData();

        data.playerId = playerId;
        data.lastSaveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");

        data.isSpawning = EnemyManager.Instance.IsSpawning();
        data.bgmStarted = DialogueManager.Instance.BgmEverStarted();

        // 从各系统收集数据
        data.score = ScoreManager.Instance.currentScore;

        PlayerHealth player=FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            data.playerHealth = player.health;
            data.playerMaxHealth = player.maxHealth;
        }

        if(LevelSystem.Instance != null)
        {
            data.level = LevelSystem.Instance.currentLevel;
            data.exp = LevelSystem.Instance.currentExp;
        }

        // 背包数据
        data.bagItems = InventoryModel.Instance.ExportAllItems();
        // 任务数据
        data.achievements = AchievementManager.Instance.ExportAchievements();

        // 设置
        data.bgmVolume = 1f;
        data.sfxVolume = 1f;
        data.resolutionWidth = Screen.width;
        data.resolutionHeight = Screen.height;

        //序列化+写入
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(GetSavePath(_currentPlayerId), json);

        Debug.Log($"存档成功 → {GetSavePath(_currentPlayerId)}");
    }

    /// <summary>
    /// 从文件读取 JSON → 反序列化 → 恢复到各系统
    /// </summary>
    public void Load(string playerId)
    {
        _currentPlayerId = playerId;
        if (!File.Exists(GetSavePath(_currentPlayerId)))
        {
            Debug.Log($"玩家 {playerId} 没有存档");
            return;
        }

        string json = File.ReadAllText(GetSavePath(_currentPlayerId));
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.isSpawning && EnemyManager.Instance != null)
         EnemyManager.Instance.StartSpawning();

         if (data.bgmStarted && DialogueManager.Instance != null)
         {
            DialogueManager.Instance.bgmSource.Play();
            DialogueManager.Instance.MarkBgmStarted();
         }

        //0. 恢复分数
        ScoreManager.Instance.currentScore = data.score;
        ScoreManager.Instance.ScoreChanged?.Invoke(data.score);

        // 1. 恢复血量
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.health = data.playerHealth;
            player.maxHealth = data.playerMaxHealth;
            player.HealthChanged?.Invoke(player.health, player.maxHealth, player.PlayerIsDeath);
        }

        // 2. 恢复等级
        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel = data.level;
            LevelSystem.Instance.currentExp = data.exp;
            LevelSystem.Instance.UpdateUI();
        }

        // 3. 恢复背包
        InventoryModel bag = InventoryModel.Instance;
        bag.ClearAllItems();
        foreach (var itemData in data.bagItems)
        {
            ItemData resource = Resources.Load<ItemData>($"Items/{itemData.itemName}");
            if (resource != null)
            {
                bag.RestoreItem(resource, itemData.slotIndex, itemData.count);
            }
            else
            {
                Debug.LogWarning($"找不到物品资源: {itemData.itemName}");
            }
        }

        //恢复任务状态
        AchievementManager.Instance.ImportAchievements(data.achievements);

        // 4. 恢复设置
        AudioListener.volume = data.sfxVolume;
        Screen.SetResolution(data.resolutionWidth, data.resolutionHeight, Screen.fullScreen);

        // 读档最后
        var npc = FindObjectOfType<NPCVisibility>();
        if (npc != null) npc.CheckVisibility();

        Debug.Log("读档成功");
    }

    /// <summary>
    /// 检查是否有存档
    /// </summary>
    public bool HasSaveFile(string playerId)
    {
        return File.Exists(GetSavePath(playerId));
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSave(string playerId)
    {
        string path = GetSavePath(playerId);
        if (File.Exists(path))
            File.Delete(path);
    }

    /// <summary>
    /// 返回所有已存在存档的预览信息（存档面板 UI 用）
    /// </summary>
    public List<SaveSlotInfo> GetAllSlotInfos()
    {
        var list = new List<SaveSlotInfo>();
        var files = Directory.GetFiles(Application.persistentDataPath, "save_*.json");
        foreach (var f in files)
        {
            string json = File.ReadAllText(f);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            list.Add(new SaveSlotInfo
            {
                playerId = data.playerId,
                isEmpty = false,
                playerLevel = data.level,
                score = data.score,
                lastSaveTime = data.lastSaveTime
            });
        }
        list.Sort((a, b) => a.playerId.CompareTo(b.playerId));
        return list;
    }
}
