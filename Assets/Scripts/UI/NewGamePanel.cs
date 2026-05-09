using UnityEngine;
using TMPro;

public class NewGamePanel : MonoBehaviour
{
    public TMP_InputField inputField;
    public TMP_Text warningText;          // "ID 已存在" 
    public GameObject oriPanel;           // 返回主菜单

    void Start()
    {
        warningText.gameObject.SetActive(false);
    }

    public void OnClickConfirm()
    {
        string id = inputField.text.Trim();
        if (string.IsNullOrEmpty(id))
        {
            warningText.text = "ID 不能为空";
            warningText.gameObject.SetActive(true);
            return;
        }

        if (SaveManager.Instance.PlayerExists(id))
        {
            warningText.text = $"ID [{id}] 已存在，请换一个";
            warningText.gameObject.SetActive(true);
            return;
        }

        // 新 ID → 进游戏
        GameManager.Instance.NewGame(id);
    }

   
}
