using UnityEngine;
using UnityEngine.EventSystems;

public class BagItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private int _slotIndex;        // 这个物品属于第几个格子
    private BagView _bagView;      // 背包主脚本（管理整个背包）

    private Transform _originalParent;  // 记住：拖拽前，我原来的父物体是谁（原来的格子）
    private Vector3 _originalPosition;  // 记住：拖拽前，我原来的位置

    private CanvasGroup _canvasGroup;   // 用来控制“能不能被鼠标检测到”
    private RectTransform _rect;        // UI位置组件

    private BagSlotView _currentHighlightSlot;// 当前高亮的格子

    public void Initialize(int slotIndex, BagView bagView)
    {
        _slotIndex = slotIndex;
        _bagView = bagView;
        _rect = GetComponent<RectTransform>();// 获取UI位置组件
        // 获取CanvasGroup，如果没有就自动加上
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_rect == null || _bagView == null) return;
        //记住原来的父物体
        _originalParent = transform.parent;
        //记住原来的位置
        _originalPosition = _rect.anchoredPosition;

        //把自己放到顶层
        transform.SetParent(_bagView.DragRoot, true);
        //让自己不被鼠标检测到（这样下面的格子才能检测到鼠标）
        _canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
         if (_rect == null) return;
        // 拖拽中的逻辑
         _rect.position = eventData.position;
        Debug.Log("正在拖拽物品");
        UpdateHighlight(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_rect == null || _bagView == null) return;  
        // 先关掉当前高亮
        if (_currentHighlightSlot != null)
        {
            _currentHighlightSlot.SetHighlight(false);
            _currentHighlightSlot = null;
        }
        
        //让自己重新被鼠标检测到
        _canvasGroup.blocksRaycasts = true;

        BagSlotView targetSlot = FindTargetSlot(eventData);
        if (targetSlot != null && targetSlot.SlotIndex != _slotIndex)
        {
            _bagView.OnSwapItemRequested?.Invoke(_slotIndex, targetSlot.SlotIndex);
            Debug.Log("交换完成");
            Destroy(gameObject);
            return;
        }

        ResetPosition();
        Debug.Log("没有找到目标格子，恢复原位");
    }

    //找到目标槽位
    private BagSlotView FindTargetSlot(PointerEventData eventData)
    {
        for (int i = eventData.hovered.Count - 1; i >= 0; i--)
        {
            GameObject hovered = eventData.hovered[i];
            if (hovered == gameObject || hovered.transform.IsChildOf(transform))
                continue;

            BagSlotView slot = hovered.GetComponentInParent<BagSlotView>();
            if (slot != null)
                return slot;
        }
        return null;
    }

    private void UpdateHighlight(PointerEventData eventData)
    {
        BagSlotView targetSlot = FindTargetSlot(eventData);

        if (_currentHighlightSlot != targetSlot)
        {
            if (_currentHighlightSlot != null)
                _currentHighlightSlot.SetHighlight(false);

            _currentHighlightSlot = targetSlot;

            if (_currentHighlightSlot != null)
                _currentHighlightSlot.SetHighlight(true);
        }
        print(_currentHighlightSlot != null ? $"高亮格子索引：{_currentHighlightSlot.SlotIndex}" : "没有高亮格子");
    }

    //恢复原来的位置
    private void ResetPosition()
    {
        transform.SetParent(_originalParent, true);
        _rect.anchoredPosition = _originalPosition;
    }
}

    
