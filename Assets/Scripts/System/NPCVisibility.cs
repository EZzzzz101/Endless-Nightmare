using UnityEngine;

public class NPCVisibility : MonoBehaviour
{
    void Start()
    {
    
    }

    public void CheckVisibility()
    {
        // 所有任务都领完了 → 消失
        if (AchievementManager.Instance != null)
        {
            bool allDone = true;
            foreach (var ach in AchievementManager.Instance.allAchievements)
            {
                var s = AchievementManager.Instance.GetStatus(ach.achievementID);
                if (s != AchievementStatus.Claimed)
                {
                    allDone = false;
                    break;
                }
            }
            gameObject.SetActive(!allDone);
        }
    }
}
