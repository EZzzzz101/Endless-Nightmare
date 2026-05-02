using UnityEngine;

public class BagManager : MonoBehaviour
{
    [Header("背包面板根节点")]
    public GameObject bagPanel;

    public static bool IsBagOpen { get; private set; }
    
    void Start()
    {
     if (bagPanel != null)
         {
            bagPanel.SetActive(false);
        }
        IsBagOpen = false;
    }

        void Update()
        {
            // 按Tab键切换背包显示
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                IsBagOpen = !IsBagOpen;
                bagPanel?.SetActive(IsBagOpen);
            }
        }
}
