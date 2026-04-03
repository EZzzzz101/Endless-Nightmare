using UnityEngine;
using UnityEngine.UI;

public class SkillUIManager : MonoBehaviour
{
    public static SkillUIManager Instance;

    [Header("UI 拖拽")]
    public Image fastChargeIcon;
    public Text fastChargeText;

    public Image flashIcon;
    public Text flashText;

    public Image boyIcon;
    public Text boyText;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        // 自动更新终极闪光（因为能量持续变化）
        UpdateUltimateUI();
    }

    // E 技能
    public void UpdateFastChargeUI()
    {
        var skill = FastChargeSkill.Instance;
        if (!skill.IsUnlocked())
        {
            fastChargeIcon.color = Color.gray;
            fastChargeText.text = "Lv" + skill.unlockLevel;
            return;
        }

        float cd = skill.GetCD();
        if (cd > 0)
        {
            fastChargeIcon.color = Color.gray;
            fastChargeText.text = cd.ToString("0.0");
        }
        else
        {
            fastChargeIcon.color = Color.white;
            fastChargeText.text = "E";
        }
    }

    // 终极闪光
    void UpdateUltimateUI()
    {
        var skill = UltimateFlashSkill.Instance;
        if (!skill.IsUnlocked())
        {
            flashIcon.color = Color.gray;
            flashText.text = "Lv" + skill.unlockLevel;
            return;
        }

        int percent = Mathf.RoundToInt(skill.currentEnergy);
        flashIcon.color = skill.currentEnergy > 0 ? Color.white : Color.gray;
        flashText.text = percent + "%";
    }

    // Q 技能
    public void UpdateBoyUI()
    {
        var skill = BoySkill.Instance;

        if (!skill.IsUnlocked())
        {
            boyIcon.color = Color.gray;
            boyText.text = "Lv" + skill.unlockLevel;
            return;
        }

        int charge = skill.GetCurrentCharge();

        if (charge > 0)
        {
            boyIcon.color = Color.white;
            boyText.text = "Q";
        }
        else
        {
            boyIcon.color = Color.gray;
            boyText.text = "No";
        }
    }
}