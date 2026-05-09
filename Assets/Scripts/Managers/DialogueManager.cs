using System;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    //单例模式
    public static DialogueManager Instance{get;private set;}

    //对话ui面板
    public GameObject dialoguePanel;

    public string CurrentNPCName { get; private set; }

    //当前节点
    public DialogueNode CurrentNode{get;private set;}

    //是否正在对话
    public bool IsTalking{get;private set;}

    private DialogueData _currentData;

    //当前播到第几句
    private int currentLineIndex = 0;

    //节点变化通知ui刷新委托
    public Action<DialogueNode,int> OnNodeChanged;

    

    // 背景音乐
    public AudioSource bgmSource;
    private bool _bgmEverStarted = false;
    public bool BgmEverStarted() => _bgmEverStarted;
    public void MarkBgmStarted() => _bgmEverStarted = true;

    void Awake()
    {
        Instance=this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 开始一段对话，外部调用
    /// </summary>
    public void StartDialogue(DialogueData dialogueData)
    {
        //判空
        if (dialogueData == null || dialogueData.nodes.Count == 0)
        {
            Debug.LogWarning("对话数据为空");
            return;
        }
        IsTalking=true;
        dialoguePanel.SetActive(true);
        CurrentNPCName = dialogueData.npcName;

        // 关音乐
        if (bgmSource != null) bgmSource.Pause();
        _currentData = dialogueData;
        
        ShowNode(FindStartingNode());
    }

 

    /// <summary>
    /// 选了一个选项如何跳转
    /// </summary>
    public void SelectOption(int optionIndex)
    {

        if (!IsTalking || CurrentNode == null) return;
        if (optionIndex < 0 || optionIndex >= CurrentNode.playerOptions.Count)
          {
              Debug.LogWarning("选项索引越界");
              return;
          }
        //去选项列表里面找索引
        DialogueOption selected = CurrentNode.playerOptions[optionIndex];

        // 如果有绑定的任务 → 接取
        if (!string.IsNullOrEmpty(selected.acceptMissionID)
            && AchievementManager.Instance != null)
        {
            AchievementManager.Instance.AcceptAchievement(selected.acceptMissionID);
        }

        // 交任务
        if (!string.IsNullOrEmpty(selected.claimMissionID)
            && AchievementManager.Instance != null)
        {
            AchievementManager.Instance.ClaimReward(selected.claimMissionID);
        }

        // （开始刷怪）
        if (selected.startSpawning && EnemyManager.Instance != null)
            EnemyManager.Instance.StartSpawning();

        // （播放BGM）
        if (selected.playBGM && bgmSource != null)
        {
            bgmSource.Play();
            _bgmEverStarted = true;
        }


        if (selected.nextNodeIndex == -1)
        {
            EndDialogue();
        }
        else
        {
            ShowNode(_currentData.nodes[selected.nextNodeIndex]);
        }
    }
    /// <summary>
    /// 结束对话
    /// </summary>
    public void EndDialogue()
    {
        IsTalking = false;
        CurrentNode = null;
        dialoguePanel.SetActive(false);
        // 恢复音乐
        if (bgmSource != null) bgmSource.UnPause();
    }

    /// <summary>
    /// 从对话的所有节点里，找第一个"条件满足"的节点作为入口。
    /// 没条件（requiredMissionID 为空）的节点永远满足，放在列表前面就能兜底。
    /// </summary>
    DialogueNode FindStartingNode()
    {
        foreach (var node in _currentData.nodes)
        {
            if (!string.IsNullOrEmpty(node.requiredMissionID))
            {
                var status = AchievementManager.Instance?.GetStatus(node.requiredMissionID);
                Debug.Log($"条件节点 [{node.nodeName}]: 要求={node.requiredStatus}, 当前状态={status}, 匹配={status == node.requiredStatus}");
            }
            
            if (!string.IsNullOrEmpty(node.requiredMissionID)
                && NodeMeetsCondition(node))
            {
                Debug.Log($"  → 命中: {node.nodeName}");
                return node;
            }
        }
        
        Debug.Log("  → 无条件兜底");
        return _currentData.nodes[0];
    }



    /// <summary>
    /// 判断一个节点的条件是否满足。
    /// 条件 = 某个任务的当前状态 == 节点要求的状态。
    /// </summary>
    bool NodeMeetsCondition(DialogueNode node)
    {
        // 没设 requiredMissionID → 无条件 → 永远满足
        if (string.IsNullOrEmpty(node.requiredMissionID))
            return true;

        // AchievementManager 还没初始化 → 安全兜底
        if (AchievementManager.Instance == null)
            return true;

        // 新增：任务已领取 → 这个条件节点永久失效，跳过
        if (AchievementManager.Instance.GetStatus(node.requiredMissionID) == AchievementStatus.Claimed)
           { Debug.Log("不用检测，永久失效");
            return false;}

        // 查：这个任务现在的状态 是不是等于 这个节点要求的状态
        // 例：节点要求 CollectItem=Completable，现在任务刚好 Completable → 匹配
        return AchievementManager.Instance.GetStatus(node.requiredMissionID)
            == node.requiredStatus;
    }


    /// <summary>
    /// 显示当前节点：更新文字 + 通知 UI
    /// </summary>      
    private void ShowNode(DialogueNode dialogueNode)
    {
        CurrentNode = dialogueNode;
        currentLineIndex = 0;
        //通知UI刷新显示
        OnNodeChanged?.Invoke(dialogueNode,currentLineIndex);
    }

    public void ContinueDialogue()
    {
        currentLineIndex++;
        if (currentLineIndex < CurrentNode.npcLines.Count)
        {
            // 还有下一句，继续播
            OnNodeChanged?.Invoke(CurrentNode, currentLineIndex);
        }
        // 播完了就停在最后一句，选项按钮自动出现（UI 判断 index 到末尾就显示按钮）
        else
        {
            // 所有句子播完 & 无选项 → 结束对话
            if (CurrentNode.playerOptions.Count == 0)
            {
                EndDialogue();
            }
        }
    }
}
