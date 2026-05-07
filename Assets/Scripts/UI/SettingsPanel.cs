using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    //拿下拉框
    public TMP_Dropdown resolutionDropdown;

    //拿主音乐
    public Slider volumeSlider;

    void Start()
    {
        // 给下拉框绑定事件
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        // 初始化音量滑条
        volumeSlider.value = AudioListener.volume;
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnResolutionChanged(int index)
    {
        // 按索引设置分辨率
        switch (index)
        {
            case 0: Screen.SetResolution(1600, 900,  Screen.fullScreen); break;
            case 1: Screen.SetResolution(1920, 1080, Screen.fullScreen); break;
            case 2: Screen.SetResolution(1280, 720,  Screen.fullScreen); break;
        }
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
}
