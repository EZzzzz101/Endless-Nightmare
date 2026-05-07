using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement Data")]
public class AchievementData : ScriptableObject
{
    public string achievementID;       // 唯一 ID，如 "collect_3_hp"
    public string title;               // 显示标题，如 "药水收藏家"
    public string description;         // 描述，如 "收集 3 个生命药水"
    public AchievementType type;       // 条件类型
    public string targetItemName;      // 目标物品名（收集型用）
    public int targetCount;            // 目标数量
    public int rewardScore;              // 奖励分数
}

// 条件类型枚举
public enum AchievementType
{
    CollectItem,   // 收集指定物品
    KillEnemy,     // 击杀指定怪物类型
    ReachLevel,    // 达到指定等级
}
