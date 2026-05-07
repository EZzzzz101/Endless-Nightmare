using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class InventoryModel
{
    private static InventoryModel _instance;
    public static InventoryModel Instance => _instance ??= new InventoryModel();

    // 背包里的所有物品
    private List<InventoryItem> _items = new List<InventoryItem>();

    // 背包格子总数（你已经生成了20个，这里就设20）
    public const int TotalSlots = 20;

    private InventoryModel() {}

    // 核心事件：当某个格子的物品变化时触发
    // 参数1：格子索引
    // 参数2：物品数据（null表示格子空了）
    // 参数3：物品数量
    public event System.Action<int, ItemData, int> OnSlotChanged;
    // 背包满了事件
    public event System.Action OnBagFull;

    //显示物品详细事件
    public event System.Action<string, int, string> OnShowItemDetail;

    // ------------------------------
    // 核心方法1：添加物品
    // ------------------------------
    public bool AddItem(ItemData itemData,int count)
    {
        // 1. 先尝试堆叠（如果物品可以堆叠）
        if (itemData.canStack)
        {
            InventoryItem existingItem = FindExistingStackableItem(itemData);
            if (existingItem != null)
            {
                // 堆叠成功
                existingItem.count += count;
                // 通知View更新这个格子
                OnSlotChanged?.Invoke(existingItem.slotIndex, existingItem.data, existingItem.count); 
                // AchievementManager.Instance?.ReportItemCollected(itemData.itemName, count);
                return true;
            }
        }

        // 2. 不能堆叠或没有找到可堆叠的，找第一个空格子
        int emptySlotIndex = FindFirstEmptySlot();
        if (emptySlotIndex == -1)
        {
            Debug.Log("背包满了！");
            OnBagFull?.Invoke();
            return false;
        }

        // 3. 创建新物品
        InventoryItem newItem = new InventoryItem
        {
            data = itemData,
            slotIndex = emptySlotIndex,
            count = count
        };

        // 4. 添加到列表
        _items.Add(newItem);

        // 5. 通知View更新
        OnSlotChanged?.Invoke(emptySlotIndex, itemData, count);

    //    //6. 成就系统：如果这个物品是成就相关的，报告给成就系统
    //    AchievementManager.Instance?.ReportItemCollected(itemData.itemName, count);

        return true;
    }

    // ------------------------------
    // 核心方法2：移除物品
    // ------------------------------
    public void RemoveItem(int slotIndex)
    {
        // 找到这个格子里的物品
        InventoryItem item = _items.Find(i => i.slotIndex == slotIndex);


        if (item != null)
        {
            item.count--;

            // 判断：数量 ≤ 0 才彻底删除物品
            if (item.count <= 0)
            {
                _items.Remove(item);
                // 通知View：这个格子空了
                OnSlotChanged?.Invoke(slotIndex, null, 0);
            }
            else
            {
                // 数量还有剩余 → 只刷新显示数量
                OnSlotChanged?.Invoke(slotIndex, item.data, item.count);
            }
        }
    }

    // ------------------------------
    // 核心方法3.使用物品
    // ------------------------------
    public void UseItem(int slotIndex)
    {
        InventoryItem item = _items.Find(i => i.slotIndex == slotIndex);
        if (item == null) return;

        item.count--;
        if (item.count <= 0)
        {
            _items.Remove(item);
            OnSlotChanged?.Invoke(slotIndex, null, 0);
        }
        else
        {
            OnSlotChanged?.Invoke(slotIndex, item.data, item.count);
        }
    }

    // ------------------------------
    // 核心方法4.展示物品详细信息
    // ------------------------------
    public void GetItemDetail(int slotIndex)
    {
        // 找到这个格子里的物品
        InventoryItem item = _items.Find(i => i.slotIndex == slotIndex);

        if (item != null)
        {
            // 通知View显示物品详细信息
            string itemName = item.data.itemName;
            int itemPrice = item.data.price;
            string itemDescription = item.data.description;

            OnShowItemDetail?.Invoke(itemName, itemPrice, itemDescription);
        }
    }

    // ------------------------------
    // 辅助方法：找第一个空格子
    // ------------------------------
    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < TotalSlots; i++)
        {
            //检查这个索引有没有被占用
            if (!_items.Exists(item => item.slotIndex == i))
            {
                return i;
            }

        }
        return -1; // 背包满了
    }

    // ------------------------------
    // 辅助方法：找可堆叠的物品
    // ------------------------------
    private InventoryItem FindExistingStackableItem(ItemData itemData)
    {
        foreach (var item in _items)
        {
            // 是同一种物品 && 可以堆叠 && 还没到最大堆叠数
            if (item.data == itemData && item.data.canStack && item.count < item.data.maxStackCount)
            {
                return item;
            }
        }
        return null;
    }
    // ------------------------------
    // 方法：交换物品
    // ------------------------------
    public void SwapItems(int fromIndex, int toIndex)
    {
        var fromItem = _items.Find(i => i.slotIndex == fromIndex);
        var toItem = _items.Find(i => i.slotIndex == toIndex);

        if (fromItem == null) return;

        if (toItem == null)
        {
            fromItem.slotIndex = toIndex;
            OnSlotChanged?.Invoke(fromIndex, null, 0);
            OnSlotChanged?.Invoke(toIndex, fromItem.data, fromItem.count);
        }
        else
        {
            fromItem.slotIndex = toIndex;
            toItem.slotIndex = fromIndex;
            OnSlotChanged?.Invoke(fromIndex, toItem.data, toItem.count);
            OnSlotChanged?.Invoke(toIndex, fromItem.data, fromItem.count);
        }
    }

    /// <summary>
    /// 导出所有背包物品（供存档用）
    /// </summary>
    public List<BagItemData> ExportAllItems()
    {
        List<BagItemData> result = new List<BagItemData>();
        foreach (var item in _items)
        {
            result.Add(new BagItemData
            {
                itemName = item.data.itemName,
                slotIndex = item.slotIndex,
                count = item.count
            });
        }
        return result;
    }

    /// <summary>
    /// 清空背包（读档前调用，避免旧数据残留）
    /// </summary>
    public void ClearAllItems()
    {
        _items.Clear();
        // 通知 View 所有格子清空
        for (int i = 0; i < TotalSlots; i++)
        {
            OnSlotChanged?.Invoke(i, null, 0);
        }
    }

    /// <summary>
    /// 读档专用：把物品恢复到指定格子（跳过自动找slot的逻辑）
    /// </summary>
    public void RestoreItem(ItemData itemData, int slotIndex, int count)
    {
       InventoryItem newItem =new InventoryItem
       {
           data=itemData,
           slotIndex=slotIndex,
           count=count
       };
       _items.Add(newItem);
       OnSlotChanged?.Invoke(slotIndex, itemData, count);
    }

    // 统计背包里某个物品的总数量（成就系统用）
    public int GetTotalCount(string itemName)
    {
        int total = 0;
        foreach (var item in _items)
        {
            if (item.data.itemName == itemName)
                total += item.count;
        }
        return total;
    }

    /// <summary>
    /// 按物品名字扣除指定数量
    /// </summary>
    public void RemoveItemByName(string itemName, int removeCount)
    {
        int remaining = removeCount;
        for (int i = _items.Count - 1; i >= 0 && remaining > 0; i--)
        {
            if (_items[i].data.itemName != itemName) continue;

            int remove = Mathf.Min(_items[i].count, remaining);
            _items[i].count -= remove;
            remaining -= remove;

            if (_items[i].count <= 0)
            {
                int slot = _items[i].slotIndex;
                _items.RemoveAt(i);
                OnSlotChanged?.Invoke(slot, null, 0);
            }
            else
            {
                OnSlotChanged?.Invoke(_items[i].slotIndex, _items[i].data, _items[i].count);
            }
        }
    }


}