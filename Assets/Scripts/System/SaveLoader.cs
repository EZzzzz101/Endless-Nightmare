using UnityEngine;

public class SaveLoader : MonoBehaviour
{
    void Start()
    {
        string id = GameManager.Instance.CurrentPlayerId;
        if (!string.IsNullOrEmpty(id) && SaveManager.Instance.HasSaveFile(id))
            SaveManager.Instance.Load(id);
    }

}
