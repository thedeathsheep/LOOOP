using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// 设置管理器
/// 负责管理游戏设置，包括音频、分辨率、全屏等设置
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("音频设置")]
    public AudioMixer audioMixer;        // 音频混合器
    public Slider masterVolumeSlider;    // 主音量滑块
    public Slider musicVolumeSlider;     // 音乐音量滑块
    public Slider sfxVolumeSlider;       // 音效音量滑块
    
    [Header("分辨率设置")]
    public Dropdown resolutionDropdown;  // 分辨率下拉菜单
    public Toggle fullscreenToggle;      // 全屏切换开关
    
    [Header("UI按钮")]
    public Button saveButton;            // 保存按钮
    public Button backButton;            // 返回按钮
    
    private Resolution[] resolutions;    // 可用分辨率数组
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void Start()
    {
        SetupAudioSliders();      // 设置音频滑块
        SetupResolutionDropdown(); // 设置分辨率下拉菜单
        SetupButtons();           // 设置按钮事件
        LoadSettings();           // 加载保存的设置
    }
    
    /// <summary>
    /// 设置音频滑块的监听事件
    /// </summary>
    private void SetupAudioSliders()
    {
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume); // 主音量变化监听
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);   // 音乐音量变化监听
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);       // 音效音量变化监听
    }
    
    /// <summary>
    /// 设置分辨率下拉菜单
    /// 获取系统支持的所有分辨率并填充到下拉菜单中
    /// </summary>
    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions; // 获取系统支持的分辨率
        resolutionDropdown.ClearOptions(); // 清空下拉菜单选项
        
        int currentResolutionIndex = 0; // 当前分辨率索引
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height; // 格式化分辨率字符串
            resolutionDropdown.options.Add(new Dropdown.OptionData(option)); // 添加到下拉菜单
            
            // 检查是否为当前分辨率
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i; // 记录当前分辨率索引
            }
        }
        
        resolutionDropdown.value = currentResolutionIndex; // 设置当前选中的分辨率
        resolutionDropdown.RefreshShownValue(); // 刷新显示
        
        // 添加监听事件
        resolutionDropdown.onValueChanged.AddListener(SetResolution); // 分辨率变化监听
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);   // 全屏切换监听
    }
    
    /// <summary>
    /// 设置按钮点击事件
    /// </summary>
    private void SetupButtons()
    {
        saveButton.onClick.AddListener(SaveSettings);     // 保存设置
        backButton.onClick.AddListener(BackToMainMenu);   // 返回主菜单
    }
    
    /// <summary>
    /// 设置主音量
    /// </summary>
    /// <param name="volume">音量值 (0-1)</param>
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20); // 设置音频混合器参数
        PlayerPrefs.SetFloat("MasterVolume", volume); // 保存到本地存储
    }
    
    /// <summary>
    /// 设置音乐音量
    /// </summary>
    /// <param name="volume">音量值 (0-1)</param>
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20); // 设置音频混合器参数
        PlayerPrefs.SetFloat("MusicVolume", volume); // 保存到本地存储
    }
    
    /// <summary>
    /// 设置音效音量
    /// </summary>
    /// <param name="volume">音量值 (0-1)</param>
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // 设置音频混合器参数
        PlayerPrefs.SetFloat("SFXVolume", volume); // 保存到本地存储
    }
    
    /// <summary>
    /// 设置分辨率
    /// </summary>
    /// <param name="resolutionIndex">分辨率索引</param>
    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex]; // 获取选中的分辨率
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen); // 设置屏幕分辨率
        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex); // 保存到本地存储
    }
    
    /// <summary>
    /// 设置全屏模式
    /// </summary>
    /// <param name="isFullscreen">是否全屏</param>
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen; // 设置全屏模式
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0); // 保存到本地存储
    }
    
    /// <summary>
    /// 保存所有设置到本地存储
    /// </summary>
    public void SaveSettings()
    {
        PlayerPrefs.Save(); // 保存所有设置
        Debug.Log("Settings saved!"); // 输出保存成功信息
    }
    
    /// <summary>
    /// 返回主菜单
    /// 查找主菜单管理器并调用显示主菜单方法
    /// </summary>
    public void BackToMainMenu()
    {
        MainMenuManager menuManager = FindObjectOfType<MainMenuManager>(); // 查找主菜单管理器
        if (menuManager != null)
        {
            menuManager.ShowMainMenu(); // 显示主菜单
        }
    }
    
    /// <summary>
    /// 加载保存的设置
    /// 从本地存储中读取设置并应用到UI组件
    /// </summary>
    private void LoadSettings()
    {
        // 加载音频设置
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f); // 获取主音量，默认1
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);   // 获取音乐音量，默认1
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);       // 获取音效音量，默认1
        
        masterVolumeSlider.value = masterVolume; // 设置主音量滑块值
        musicVolumeSlider.value = musicVolume;   // 设置音乐音量滑块值
        sfxVolumeSlider.value = sfxVolume;       // 设置音效音量滑块值
        
        // 加载分辨率设置
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0); // 获取分辨率索引，默认0
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;     // 获取全屏设置，默认开启
        
        if (resolutionIndex < resolutions.Length)
        {
            resolutionDropdown.value = resolutionIndex; // 设置分辨率下拉菜单值
            resolutionDropdown.RefreshShownValue();     // 刷新显示
        }
        
        fullscreenToggle.isOn = fullscreen; // 设置全屏开关状态
    }
} 