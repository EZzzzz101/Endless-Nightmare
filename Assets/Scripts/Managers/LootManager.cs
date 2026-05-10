using UnityEngine;

public class LootManager : MonoBehaviour
{
   public static LootManager Instance { get; private set; }

    [Header("掉落物预制体")]
    public GameObject pickupPrefab;

    [Header("角色碰撞器")]
    public SphereCollider playerPickupCollider;

    void Awake()
    {
        Instance = this;
        // 初始化对象池
    }

    /// <summary>
    /// 怪物死亡时调用：传入怪物类型对应的掉落表，返回实际掉落的物品列表
    /// </summary>
    public void RollAndSpawn(Vector3 deathPosition,DropTable dropTable)
    {
        if (dropTable==null) return;

        //随机抽一个物品
        DropEntry entry=RollEntry(dropTable);
        if (entry==null||entry.item==null)  return;
       
        // 随机数量
        int count = Random.Range(entry.minCount, entry.maxCount + 1);

        //从对象池取出在死亡位置生成
        GameObject newPickUpObj = ObjectPool.Instance.GetFromPool("pickup", deathPosition, Quaternion.identity);
        PickUpItem pickup = newPickUpObj.GetComponent<PickUpItem>();
        pickup?.SetItem(entry.item, count);
    }

    // 根据权重随机抽取一个掉落项
    private DropEntry RollEntry(DropTable table)
    {
        float totalWeight = 0;
        foreach (var e in table.entries)
            totalWeight += e.weight;

        float roll = Random.Range(0, totalWeight);
        float cursor = 0;
        foreach (var e in table.entries)
        {
            cursor += e.weight;
            if (roll <= cursor)
                return e;
        }
        return null;
    }

    public bool TryPickup(ItemData item,int count)
    {
        InventoryModel model = InventoryModel.Instance;
        // 尝试添加到背包，成功返回 true
        bool success= model.AddItem(item, count);
        print($"尝试拾取 {item.itemName} x{count}，结果: {(success ? "成功" : "失败")}");
        return success;
    }
}
