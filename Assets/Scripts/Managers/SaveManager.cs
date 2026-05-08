using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance{get;private set;}

    // 存档文件路径
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, "save_data.json");

    void Awake()
    {
        Instance=this;
 
    }
    /// <summary>
    /// 从各系统收集数据 → 序列化为 JSON → 写入文件
    /// </summary>
    public void Save()
    {
        SaveData data = new SaveData();

        data.isSpawning = EnemyManager.Instance.IsSpawning();
        data.bgmStarted = DialogueManager.Instance.BgmEverStarted();


        // 从各系统收集数据
        //分数
        data.score = ScoreManager.Instance.currentScore;
        //玩家血量
        PlayerHealth player=FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            data.playerHealth = player.health;
            data.playerMaxHealth = player.maxHealth;
        }
        
        //等级
        if(LevelSystem.Instance != null)
        {
            data.level = LevelSystem.Instance.currentLevel;
            data.exp = LevelSystem.Instance.currentExp;
        }

        // 背包数据
        data.bagItems = InventoryModel.Instance.ExportAllItems();
        // 任务数据
        data.achievements = AchievementManager.Instance.ExportAchievements();


        // 设置
        data.bgmVolume = 1f;  // 后面接了 Slider 再替换
        data.sfxVolume = 1f;
        data.resolutionWidth = Screen.width;
        data.resolutionHeight = Screen.height;
        
        //序列化+写入
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SaveFilePath, json);

        Debug.Log($"存档成功 → {SaveFilePath}");
    }

    /// <summary>
    /// 从文件读取 JSON → 反序列化 → 恢复到各系统
    /// </summary>
    public void Load()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("没有存档文件，跳过读取");
            return;
        }

        string json = File.ReadAllText(SaveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data.isSpawning && EnemyManager.Instance != null)
         EnemyManager.Instance.StartSpawning();

         if (data.bgmStarted && DialogueManager.Instance != null)
         DialogueManager.Instance.bgmSource.Play();


        //0. 恢复分数
        ScoreManager.Instance.currentScore = data.score;
        ScoreManager.Instance.ScoreChanged?.Invoke(data.score); // 手动触发事件，刷新

        // 1. 恢复血量
        PlayerHealth player = FindObjectOfType<PlayerHealth>();
        if (player != null)
        {
            player.health = data.playerHealth;
            player.maxHealth = data.playerMaxHealth;
            // 手动触发通知，让 HealthUI 刷新
            player.HealthChanged?.Invoke(player.health, player.maxHealth, player.PlayerIsDeath);
        }

        // 2. 恢复等级
        if (LevelSystem.Instance != null)
        {
            LevelSystem.Instance.currentLevel = data.level;
            LevelSystem.Instance.currentExp = data.exp;

            LevelSystem.Instance.UpdateUI();   
        }

        // 3. 恢复背包
        InventoryModel bag = InventoryModel.Instance;
        bag.ClearAllItems();
        foreach (var itemData in data.bagItems)
        {
            // 通过名字找回 ItemData 资源
            ItemData resource = Resources.Load<ItemData>($"Items/{itemData.itemName}");
            if (resource != null)
            {
                // 直接构造 InventoryItem 塞进去（不用 AddItem，因为它会自己找 slot）
                // 这里用 AddItem 的堆叠逻辑即可
                bag.RestoreItem(resource, itemData.slotIndex, itemData.count);
            }
            else
            {
                Debug.LogWarning($"找不到物品资源: {itemData.itemName}");
            }
        }

        //恢复任务状态
        AchievementManager.Instance.ImportAchievements(data.achievements);

        // 4. 恢复设置
        AudioListener.volume = data.sfxVolume;
        Screen.SetResolution(data.resolutionWidth, data.resolutionHeight, Screen.fullScreen);

        // 分辨率 ... 后续接上
       

        Debug.Log("读档成功");
    }

    /// <summary>
    /// 检查是否有存档
    /// </summary>
    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
            File.Delete(SaveFilePath);
    }
}
