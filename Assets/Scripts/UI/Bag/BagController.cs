using UnityEngine;

public class BagController : MonoBehaviour
{

    private BagView _view;
    private InventoryModel _model;



    void Awake()
    {
       

        // 1. 获取View组件
        _view = GetComponent<BagView>();

        // 2. 实例化Model
        _model = InventoryModel.Instance;
        // 3. 订阅Model的事件：Model数据变化 → View更新显示
        _model.OnSlotChanged += _view.UpdateSlot;

        //订阅移除按钮
        _view.OnRemoveItemClicked += _model.RemoveItem;

        //获得物品详细事件
        _view.OnGetItemDetail += _model.GetItemDetail;

        //展示物品详细事件
        _model.OnShowItemDetail += _view.ShowItemDetail;

        //订阅背包满了事件
        _model.OnBagFull += _view.ShowTip;

        //订阅物品交换事件
        _view.OnSwapItemRequested += _model.SwapItems;

        // ------------------------------
        // 测试代码：游戏开始时自动加3个物品
        // 后面会删掉，换成商店购买
        // ------------------------------
        //TestAddItems();
    }

    // ------------------------------
    // 公共方法：给外部调用的接口（比如商店）
    // ------------------------------
    public bool AddItemToBag(ItemData itemData,int count)
    {
        return _model.AddItem(itemData,count);
    }


    // ------------------------------
    // 测试方法
    // ------------------------------
    // private void TestAddItems()
    // {
    //    // 加载你之前创建的物品数据（放在Resources/Items文件夹下）
    //    ItemData healthPotion = Resources.Load<ItemData>("Items/HealthPotion");
    //    ItemData manaPotion = Resources.Load<ItemData>("Items/ManaPotion");
    //    ItemData weapon = Resources.Load<ItemData>("Items/Weapon");

    //    // 测试添加
    //    if (healthPotion != null)
    //    {
    //        _model.AddItem(healthPotion,1);
    //        _model.AddItem(healthPotion,1); // 测试堆叠
    //    }
    //    if (manaPotion != null)
    //    {
    //        _model.AddItem(manaPotion,1);
    //    }
    //    if (weapon != null)
    //    {
    //        _model.AddItem(weapon,1);
    //    }
    //    print("测试1");
    // }

    void OnDestroy()
    {
        // 取消订阅事件
        if (_model != null)
        {
            _model.OnSlotChanged -= _view.UpdateSlot;
            _view.OnRemoveItemClicked -= _model.RemoveItem;
            _view.OnGetItemDetail -= _model.GetItemDetail;
            _model.OnShowItemDetail -= _view.ShowItemDetail;
            _model.OnBagFull -= _view.ShowTip;
        }
    }
}