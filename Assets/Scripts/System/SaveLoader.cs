using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    void Start()
    {
        // 如果有存档就读取
        if (SaveManager.Instance.HasSaveFile())
        {
            SaveManager.Instance.Load();
        }
        else
        {
            Debug.Log("没有存档，开始新游戏");
            // 可以在这里初始化一些默认数据
        }
    }
}
