using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotPanel : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform content;

    void OnEnable()
    {
        Refresh();
    }

    void Refresh()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);

        var infos = SaveManager.Instance.GetAllSlotInfos();

        foreach (var info in infos)
        {
            GameObject block = Instantiate(slotPrefab, content);

            block.transform.Find("level").GetComponent<TMP_Text>().text
                = $"Lv.{info.playerLevel}";
            block.transform.Find("id").GetComponent<TMP_Text>().text
                = info.playerId;
            block.transform.Find("time").GetComponent<TMP_Text>().text
                = info.lastSaveTime;
            block.transform.Find("score").GetComponent<TMP_Text>().text
                = $"分数: {info.score}";

            string id = info.playerId;

            // 点击空白区 → 读档
            var mainBtn = block.transform.Find("LoadBtn").GetComponent<Button>();
            mainBtn.onClick.AddListener(() =>
            {
                GameManager.Instance.ContinueGame(id);
            });

            // 删除按钮
            var delBtn = block.transform.Find("RemoveBtn").GetComponent<Button>();
            delBtn.onClick.AddListener(() =>
            {
                SaveManager.Instance.DeleteSave(id);
                Refresh();
            });
        }
    }
}
