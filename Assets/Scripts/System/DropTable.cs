using System;
using UnityEngine;

[CreateAssetMenu(menuName = "掉落/掉落表")]
public class DropTable : ScriptableObject
{
    public DropEntry[] entries;
}

[Serializable]
public class DropEntry
{
    public ItemData item;     // 掉落什么物品（引用已有的 ItemData）

    public float weight;      // 权重，比如 10，越高越容易掉
    public int minCount = 1;  // 最小掉落数量
    public int maxCount = 1;  // 最大掉落数量
}
