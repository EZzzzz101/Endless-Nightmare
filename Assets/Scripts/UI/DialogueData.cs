using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="对话/对话数据")]
public class DialogueData : ScriptableObject
{
    public string npcName;          // NPC名称
    public List<DialogueNode> nodes = new List<DialogueNode>();
}
/// <summary>
/// 对话树的一个节点（NPC 说的一句话 + 玩家的可选回复）
/// </summary>
[Serializable]
public class DialogueNode
{
    public string nodeName;         //节点名字
    public List<string> npcLines = new List<string>();//npc持续对话
    public List<DialogueOption> playerOptions=new List<DialogueOption>(); // 玩家可选回复列表
}

[System.Serializable]
public class DialogueOption
{
    public string playerText; // 回复文字
    public int nextNodeIndex = -1;   // -1 = 对话结束
}
