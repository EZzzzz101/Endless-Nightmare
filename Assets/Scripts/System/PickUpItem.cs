using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    public ItemData itemData; // 物品数据
    public int count;       // 物品数量

    //音效 
    public AudioClip pickupClip;


    public void SetItem(ItemData data, int cnt)
    {
        itemData = data;
        count = cnt;

        // 可以在这里根据物品类型换模型/颜色（没有 3D 模型就先不改）
    }

    //拾取方法
    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("ItemCollector"))
    {
        bool success = LootManager.Instance.TryPickup(itemData, count);
        if (success)
        {
            // 音效不在掉落物上播，换成在角色那里播
            AudioSource.PlayClipAtPoint(pickupClip, transform.position);
            
            // 立刻回收，不等
            gameObject.SetActive(false);
        }
    }
}

}
