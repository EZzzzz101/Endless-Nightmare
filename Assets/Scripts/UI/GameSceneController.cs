using UnityEngine;

public class GameSceneController : MonoBehaviour
{
    public void OnClickSaveGame()
    {
        //保存游戏数据并返回主界面
        //保存游戏数据环节
        Debug.Log($"GameManager.Instance 是否为 null：{GameManager.Instance == null}");
        SaveManager.Instance.Save();
        GameManager.Instance.LoadMainScene();
    }

    //跳转到设置
    public void OnClickSettings()
    {
        //打开设置界面
        GameUIManager.Instance.ToSettings();

    }
}
