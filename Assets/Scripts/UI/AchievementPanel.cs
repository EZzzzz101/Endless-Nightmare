using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class AchievementPanel : MonoBehaviour
{
    public GameObject achievementBlockPrefab;   // 成就行预制体
    public Transform content;                    // ScrollView 的 Content

    //获取AchievementManager.Instance
    private AchievementManager _achievementManager;
    // void Awake()
    // {
    //     _achievementManager = AchievementManager.Instance;
    //     _achievementManager.OnProgressUpdated += Refresh;
    // }

    void OnEnable()
    {
        if (_achievementManager == null)
        _achievementManager = AchievementManager.Instance;

        _achievementManager.OnProgressUpdated += Refresh;
        Refresh();
    }

    void OnDestroy()
    {
        _achievementManager.OnProgressUpdated -= Refresh;
    }

    void Refresh()
    {

        // 1. 清空旧行
        foreach (Transform child in content)
            Destroy(child.gameObject);

        // 2. 遍历所有成就，每一行一个 Block
        foreach (var ach in _achievementManager.allAchievements)
        {

            var status = _achievementManager.GetStatus(ach.achievementID);

            // 未接取或已领取 → 跳过
            if (status == AchievementStatus.NotAccepted || status == AchievementStatus.Claimed)
                continue;

            //接取了才显示   
            GameObject block = Instantiate(achievementBlockPrefab, content);
            // 标题
            var titleText = block.transform.Find("TitleText").GetComponent<TMP_Text>();
            titleText.text = ach.title;

            //描述文字
            var descText = block.transform.Find("DescriptionText").GetComponent<TMP_Text>();
            descText.text=ach.description;
            // 进度文字
            int current = _achievementManager.GetProgress(ach.achievementID);
            var progressText = block.transform.Find("ProgressText").GetComponent<TMP_Text>();
            progressText.text = $"{Mathf.Min(current,ach.targetCount)} / {ach.targetCount}";

            //任务报酬


            // // 领取按钮
            // var claimBtn = block.transform.Find("ClaimButton").GetComponent<Button>();
            // var btnText = claimBtn.GetComponentInChildren<TMP_Text>();

            // //1.可领取状态
            // if (status== AchievementStatus.Completable)
            // {
            //     claimBtn.interactable = true;
            //     btnText.text = "领取";
            // }
            // //2.未完成状态
            // else
            // {
            //     claimBtn.interactable = false;
            //     btnText.text = "未完成";
            // }

            // // 闭包
            // string id = ach.achievementID;
            // claimBtn.onClick.AddListener(() =>
            // {
            //     _achievementManager.ClaimReward(id);
            // });
        }
    }
}
