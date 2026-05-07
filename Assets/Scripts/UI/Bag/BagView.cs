using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;


public class BagView : MonoBehaviour
{
    [Header("引用对象")]
    public GameObject SlotPrefab;   // 格子槽预制体
    public Transform SlotContainer;  // 格子槽容器
    public GameObject ItemPrefab;    // 物品预制体
    public Text tipText;// 提示文本预制体

    [Header("详细数据预制体")]
    public GameObject ItemDetailPrefab; // 物品详细数据预制体
    // public Text detailName; // 物品名称文本

    // public Text detailPrice;// 物品价格文本

    // public Text detailDescription;// 物品描述文本

    public Transform DragRoot;//顶层拖拽根节点

    private GameObject _detailPanelInstance;// 物品详细数据面板实例


    // 格子槽数组，用于存储所有格子槽对象
    private GameObject[] _slots = new GameObject[InventoryModel.TotalSlots];

    //移除事件
    public System.Action<int> OnRemoveItemClicked;

    //活动物品详细事件
    public System.Action<int> OnGetItemDetail;

    //物品交换请求事件
    public System.Action<int, int> OnSwapItemRequested;

    void Awake()
    {
        if (ItemDetailPrefab != null)
        {
            _detailPanelInstance = Instantiate(ItemDetailPrefab, transform);
            _detailPanelInstance.SetActive(false);
        }
        // 生成20个格子槽
        GenerateSlots();
        print("BagView Awake完成");
    }

    void Start()
    {
        tipText.gameObject.SetActive(false);   
    }

    // ------------------------------
    // 批量生成格子槽
    // ------------------------------
    private void GenerateSlots()
    {
        for (int i = 0; i < InventoryModel.TotalSlots; i++)
        {
            // 实例化格子槽
            GameObject slot = Instantiate(SlotPrefab, SlotContainer);
            // 设置格子槽名称
            slot.name = $"Slot_{i}";
            // 将格子槽添加到数组中
            _slots[i] = slot;
             BagSlotView slotView = slot.GetComponent<BagSlotView>();
            if (slotView != null)
            {
                slotView.SlotIndex = i;
            }
        }
    }

    // ------------------------------
    // 更新格子槽内容
    // ------------------------------
    public void UpdateSlot(int slotIndex, ItemData itemData, int count)
    {
        // 1. 验证格子索引是否在有效范围内
        if (slotIndex < 0 || slotIndex >= _slots.Length)
        {
            Debug.LogError($"格子索引超出范围：{slotIndex}，有效范围为0~19");
            return;
        }

        // 2. 获取对应的格子槽对象
        GameObject slot = _slots[slotIndex];

        // 3. 清空格子槽内容（如果有的话）
        if (slot!= null)
        {
            ClearSlot(slot);
        }

        // 4. 实例化物品
        if (itemData != null)
        {
            SpawnItemInSlot(slot, itemData, count, slotIndex);
        }
    }

    // ------------------------------
    // 清空格子槽内容
    // ------------------------------
    private void ClearSlot(GameObject slot)
    {
        foreach (Transform child in slot.transform)
        {
            // 只删除Tag为"Item"的子对象，避免误删其他UI元素
            if (child.CompareTag("Item"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    // ------------------------------
    // 在格子槽中生成物品
    // ------------------------------
    private void SpawnItemInSlot(GameObject slot, ItemData itemData, int count, int slotIndex)
    {
        // 1. 实例化物品预制体
        GameObject item = Instantiate(ItemPrefab, slot.transform);

        // 2. 设置物品位置
        RectTransform rect = item.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        // 3. 设置物品图标
        Image iconImage = item.GetComponent<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = itemData.icon;
        }

        // 4. 设置物品数量文本
        TextMeshProUGUI countText = item.transform.Find("CountText")?.GetComponent<TextMeshProUGUI>();
        if (countText != null)
        {
            countText.text = count > 1 ? count.ToString() : "";
        }
        // ==============================================
        // 设置删除按钮事件
        // ==============================================
        // 1. 查找物品上的删除按钮
        Button removeBtn = item.transform.Find("RemoveBtn")?.GetComponent<Button>();

        // 2. 绑定删除按钮事件
        removeBtn.onClick.AddListener(() =>
        {
            // 调用删除物品点击事件，传递格子索引
            OnRemoveItemClicked?.Invoke(slotIndex);
        });

        // ==============================================
        // 设置物品详细事件
        // ==============================================
        // 1. 查找物品上的详细按钮
        Button detailBtn = item.transform.Find("DetailBtn")?.GetComponent<Button>();

        // 2. 绑定详细按钮事件
        detailBtn.onClick.AddListener(() =>
        {
            // 调用取数据事件，传递物品数据
            OnGetItemDetail?.Invoke(slotIndex);
            print("点击了物品详细按钮，格子索引：" + slotIndex);
        });

         // ==============================================
        // 拖拽事件
        // ==============================================
        //初始化拖拽
        BagItemDragHandler dragHandler = item.AddComponent<BagItemDragHandler>();
        if (dragHandler != null)
        {
            dragHandler.Initialize(slotIndex, this);
        }

        // BagSlotView sv = slot.GetComponent<BagSlotView>();
        // if (sv != null && sv.HighlightMask != null && sv.HighlightMask.activeSelf)
        // {
        //     sv.HighlightMask.transform.SetAsLastSibling();
        // }
 }

    //展示文本
    public void ShowTip()
    {
        StopAllCoroutines();
        tipText.gameObject.SetActive(true);
        // 启动携程，2秒后隐藏提示文本
        StartCoroutine(HideTipAfterDelay(2f));
    }

    IEnumerator HideTipAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tipText.gameObject.SetActive(false);
    }

    //显示物品详细信息窗口方法
    public void ShowItemDetail(string name, int price, string description)
    {
        // 1.激活已失活的物品详细数据面板
        if (_detailPanelInstance == null) return;
        _detailPanelInstance.SetActive(true);

        // 2. 设置物品详细信息文本
        Text detailNameText =  _detailPanelInstance.transform.Find("DetailName")?.GetComponent<Text>();
        Text detailPriceText =  _detailPanelInstance.transform.Find("DetailPrice")?.GetComponent<Text>();
        Text detailDescriptionText = _detailPanelInstance.transform.Find("DetailDescription")?.GetComponent<Text>();

        if (detailNameText != null) detailNameText.text = $"名称: {name}";
        if (detailPriceText != null) detailPriceText.text = $"价格: {price}";
        if (detailDescriptionText != null) detailDescriptionText.text = $"描述: {description}";

        // 3. 设置关闭按钮事件
        Button closeBtn = _detailPanelInstance.transform.Find("CloseBtn")?.GetComponent<Button>();
         if (closeBtn != null)
            closeBtn.onClick.AddListener(() => 
            {  if (_detailPanelInstance != null)
        _detailPanelInstance.SetActive(false); }
        );
    }
}