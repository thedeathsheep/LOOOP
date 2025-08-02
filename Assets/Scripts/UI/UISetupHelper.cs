using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI设置助手
/// 用于快速搭建Menu场景的UI结构
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("预制体引用")]
    public GameObject panelPrefab;      // 面板预制体
    public GameObject buttonPrefab;     // 按钮预制体
    public GameObject textPrefab;       // 文本预制体
    public GameObject sliderPrefab;     // 滑块预制体
    public GameObject dropdownPrefab;   // 下拉菜单预制体
    public GameObject togglePrefab;     // 开关预制体
    
    [Header("设置")]
    public Color buttonNormalColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color buttonHoverColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color buttonPressedColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    
    /// <summary>
    /// 创建主菜单面板
    /// </summary>
    public GameObject CreateMainMenuPanel()
    {
        GameObject panel = CreatePanel("MainMenuPanel");
        panel.transform.SetParent(transform);
        
        // 创建标题
        GameObject title = CreateText("Title", "LOOOP", 48, TextAnchor.MiddleCenter);
        title.transform.SetParent(panel.transform);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 200);
        
        // 创建按钮容器
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(panel.transform);
        buttonContainer.AddComponent<VerticalLayoutGroup>();
        buttonContainer.GetComponent<VerticalLayoutGroup>().spacing = 20;
        buttonContainer.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
        buttonContainer.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
        buttonContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        
        // 创建按钮
        CreateButton("StartGameButton", "开始游戏", buttonContainer.transform);
        CreateButton("ContinueGameButton", "继续游戏", buttonContainer.transform);
        CreateButton("SettingsButton", "设置", buttonContainer.transform);
        CreateButton("CreditsButton", "制作人员", buttonContainer.transform);
        CreateButton("ExitButton", "退出游戏", buttonContainer.transform);
        
        return panel;
    }
    
    /// <summary>
    /// 创建设置面板
    /// </summary>
    public GameObject CreateSettingsPanel()
    {
        GameObject panel = CreatePanel("SettingsPanel");
        panel.transform.SetParent(transform);
        
        // 创建标题
        GameObject title = CreateText("SettingsTitle", "设置", 36, TextAnchor.MiddleCenter);
        title.transform.SetParent(panel.transform);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);
        
        // 创建设置容器
        GameObject settingsContainer = new GameObject("SettingsContainer");
        settingsContainer.transform.SetParent(panel.transform);
        settingsContainer.AddComponent<VerticalLayoutGroup>();
        settingsContainer.GetComponent<VerticalLayoutGroup>().spacing = 30;
        settingsContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 50);
        
        // 音频设置
        CreateAudioSettings(settingsContainer.transform);
        
        // 图形设置
        CreateGraphicsSettings(settingsContainer.transform);
        
        // 按钮
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(panel.transform);
        buttonContainer.AddComponent<HorizontalLayoutGroup>();
        buttonContainer.GetComponent<HorizontalLayoutGroup>().spacing = 20;
        buttonContainer.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -200);
        
        CreateButton("SaveButton", "保存", buttonContainer.transform);
        CreateButton("BackButton", "返回", buttonContainer.transform);
        
        return panel;
    }
    
    /// <summary>
    /// 创建制作人员面板
    /// </summary>
    public GameObject CreateCreditsPanel()
    {
        GameObject panel = CreatePanel("CreditsPanel");
        panel.transform.SetParent(transform);
        
        // 创建标题
        GameObject title = CreateText("CreditsTitle", "制作人员", 36, TextAnchor.MiddleCenter);
        title.transform.SetParent(panel.transform);
        title.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 250);
        
        // 创建滚动视图
        GameObject scrollView = CreateScrollView("CreditsScrollView");
        scrollView.transform.SetParent(panel.transform);
        scrollView.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        scrollView.GetComponent<RectTransform>().sizeDelta = new Vector2(600, 400);
        
        // 创建返回按钮
        CreateButton("BackButton", "返回", panel.transform);
        GameObject backButton = panel.transform.Find("BackButton").gameObject;
        backButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250);
        
        return panel;
    }
    
    /// <summary>
    /// 创建面板
    /// </summary>
    private GameObject CreatePanel(string name)
    {
        GameObject panel = new GameObject(name);
        panel.AddComponent<RectTransform>();
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();
        panel.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        return panel;
    }
    
    /// <summary>
    /// 创建按钮
    /// </summary>
    private GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent);
        
        // 添加按钮组件
        button.AddComponent<RectTransform>();
        button.AddComponent<Image>();
        button.AddComponent<Button>();
        
        // 设置按钮颜色
        ColorBlock colors = button.GetComponent<Button>().colors;
        colors.normalColor = buttonNormalColor;
        colors.highlightedColor = buttonHoverColor;
        colors.pressedColor = buttonPressedColor;
        button.GetComponent<Button>().colors = colors;
        
        // 设置按钮大小
        button.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
        
        // 创建按钮文本
        GameObject buttonText = CreateText(name + "Text", text, 24, TextAnchor.MiddleCenter);
        buttonText.transform.SetParent(button.transform);
        buttonText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        
        return button;
    }
    
    /// <summary>
    /// 创建文本
    /// </summary>
    private GameObject CreateText(string name, string text, int fontSize, TextAnchor alignment)
    {
        GameObject textObj = new GameObject(name);
        textObj.AddComponent<RectTransform>();
        textObj.AddComponent<CanvasRenderer>();
        textObj.AddComponent<Text>();
        
        Text textComponent = textObj.GetComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = alignment;
        textComponent.color = Color.white;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        
        return textObj;
    }
    
    /// <summary>
    /// 创建滚动视图
    /// </summary>
    private GameObject CreateScrollView(string name)
    {
        GameObject scrollView = new GameObject(name);
        scrollView.AddComponent<RectTransform>();
        scrollView.AddComponent<ScrollRect>();
        scrollView.AddComponent<Image>();
        scrollView.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);
        
        // 创建Viewport
        GameObject viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform);
        viewport.AddComponent<RectTransform>();
        viewport.AddComponent<Image>();
        viewport.AddComponent<Mask>();
        viewport.GetComponent<Image>().color = new Color(0, 0, 0, 0.3f);
        
        // 创建Content
        GameObject content = new GameObject("Content");
        content.transform.SetParent(viewport.transform);
        content.AddComponent<RectTransform>();
        content.AddComponent<VerticalLayoutGroup>();
        content.GetComponent<VerticalLayoutGroup>().spacing = 10;
        content.GetComponent<VerticalLayoutGroup>().padding = new RectOffset(20, 20, 20, 20);
        
        // 设置ScrollRect
        ScrollRect scrollRect = scrollView.GetComponent<ScrollRect>();
        scrollRect.viewport = viewport.GetComponent<RectTransform>();
        scrollRect.content = content.GetComponent<RectTransform>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        
        // 添加制作人员文本
        GameObject creditsText = CreateText("CreditsText", GetCreditsContent(), 18, TextAnchor.UpperLeft);
        creditsText.transform.SetParent(content.transform);
        creditsText.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 800);
        
        return scrollView;
    }
    
    /// <summary>
    /// 创建音频设置
    /// </summary>
    private void CreateAudioSettings(Transform parent)
    {
        // 主音量
        CreateSliderSetting("MasterVolume", "主音量", parent);
        // 音乐音量
        CreateSliderSetting("MusicVolume", "音乐音量", parent);
        // 音效音量
        CreateSliderSetting("SFXVolume", "音效音量", parent);
    }
    
    /// <summary>
    /// 创建图形设置
    /// </summary>
    private void CreateGraphicsSettings(Transform parent)
    {
        // 分辨率
        CreateDropdownSetting("Resolution", "分辨率", parent);
        // 全屏
        CreateToggleSetting("Fullscreen", "全屏", parent);
    }
    
    /// <summary>
    /// 创建滑块设置
    /// </summary>
    private void CreateSliderSetting(string name, string label, Transform parent)
    {
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent);
        container.AddComponent<HorizontalLayoutGroup>();
        container.GetComponent<HorizontalLayoutGroup>().spacing = 20;
        
        // 标签
        GameObject labelObj = CreateText(name + "Label", label, 20, TextAnchor.MiddleLeft);
        labelObj.transform.SetParent(container.transform);
        labelObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        
        // 滑块
        GameObject slider = new GameObject(name + "Slider");
        slider.transform.SetParent(container.transform);
        slider.AddComponent<RectTransform>();
        slider.AddComponent<Slider>();
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);
    }
    
    /// <summary>
    /// 创建下拉菜单设置
    /// </summary>
    private void CreateDropdownSetting(string name, string label, Transform parent)
    {
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent);
        container.AddComponent<HorizontalLayoutGroup>();
        container.GetComponent<HorizontalLayoutGroup>().spacing = 20;
        
        // 标签
        GameObject labelObj = CreateText(name + "Label", label, 20, TextAnchor.MiddleLeft);
        labelObj.transform.SetParent(container.transform);
        labelObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        
        // 下拉菜单
        GameObject dropdown = new GameObject(name + "Dropdown");
        dropdown.transform.SetParent(container.transform);
        dropdown.AddComponent<RectTransform>();
        dropdown.AddComponent<Dropdown>();
        dropdown.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);
    }
    
    /// <summary>
    /// 创建开关设置
    /// </summary>
    private void CreateToggleSetting(string name, string label, Transform parent)
    {
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent);
        container.AddComponent<HorizontalLayoutGroup>();
        container.GetComponent<HorizontalLayoutGroup>().spacing = 20;
        
        // 标签
        GameObject labelObj = CreateText(name + "Label", label, 20, TextAnchor.MiddleLeft);
        labelObj.transform.SetParent(container.transform);
        labelObj.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 30);
        
        // 开关
        GameObject toggle = new GameObject(name + "Toggle");
        toggle.transform.SetParent(container.transform);
        toggle.AddComponent<RectTransform>();
        toggle.AddComponent<Toggle>();
        toggle.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
    }
    
    /// <summary>
    /// 获取制作人员内容
    /// </summary>
    private string GetCreditsContent()
    {
        return @"LOOOP Game Credits

Game Development Team:
- Lead Developer: [Your Name]
- Game Designer: [Designer Name]
- Artist: [Artist Name]
- Sound Designer: [Sound Designer Name]

Special Thanks:
- Unity Technologies
- All playtesters and supporters
- Friends and family for their patience

Technical Credits:
- Engine: Unity 2022.3 LTS
- Programming Language: C#
- Audio: Unity Audio System
- Graphics: Unity URP

© 2025 LOOOP Development Team
All rights reserved.

Thank you for playing!";
    }
} 