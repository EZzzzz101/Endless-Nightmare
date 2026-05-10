using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    void Start()
    {
        string id = GameManager.Instance.CurrentPlayerId;
       if (!string.IsNullOrEmpty(id) && SaveManager.Instance.HasSaveFile(id))
    {
        SaveManager.Instance.Load(id);
    }
    else
    {
        // 新游戏 → 清空上一局残留的内存数据
        InventoryModel.Instance.ClearAllItems();
    }
    }

}
