using UnityEngine;

// 右键菜单：Create → Shop → Item
[CreateAssetMenu(fileName = "New Item", menuName = "Shop/Item")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemName;    // 物品名称
    public Sprite icon;         // 物品图标
    public int price;           // 物品价格
    
    public string description;  // 物品描述

    [Header("背包信息")]
    public bool canStack;       // 是否可以堆叠
    public int maxStackCount;   // 最大堆叠数量
}