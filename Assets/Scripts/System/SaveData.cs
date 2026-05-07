using UnityEngine;
using System;
using System.Collections.Generic;

//可序列化
[Serializable]
public class SaveData
{
    //分数
    public int score;
    //玩家状态
    public float playerHealth;
    public float playerMaxHealth;
    //等级
    public int level;
    public float exp;
    //背包物品
    public List<BagItemData> bagItems = new List<BagItemData>();

    // 设置
    public float bgmVolume;
    public float sfxVolume;
    public int resolutionWidth;
    public int resolutionHeight;
}

// 背包物品的序列化形式（不存 ScriptableObject）
[Serializable]
public class BagItemData
{
    public string itemName;   // 用名字指向 ItemData
    public int slotIndex;
    public int count;
}
