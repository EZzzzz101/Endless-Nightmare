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
        
        ShowNode(dialogueData.nodes[0]);
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
