using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// UI设置助手
/// 用于在编辑器中快速创建和设置完整的UI界面
/// </summary>
public class UISetupHelper : MonoBehaviour
{
    [Header("UI创建选项")]
    public bool createMainMenuUI = true;    // 是否创建主菜单UI
    public bool createSettingsUI = true;    // 是否创建设置UI
    public bool createCreditsUI = true;     // 是否创建制作人员UI
    public bool createLoadingUI = true;     // 是否创建加载UI
    
    [Header("UI引用")]
    public Canvas mainCanvas;               // 主画布
    public MainMenuManager mainMenuManager; // 主菜单管理器
    public SettingsManager settingsManager; // 设置管理器
    public CreditsManager creditsManager;   // 制作人员管理器
    public UIManager uiManager;             // UI管理器
    
    #if UNITY_EDITOR
    /// <summary>
    /// 设置完整UI系统
    /// 右键菜单选项，用于快速创建完整的UI界面
    /// </summary>
    [ContextMenu("Setup Complete UI")]
    public void SetupCompleteUI()
    {
        if (createMainMenuUI) CreateMainMenuUI();    // 创建主菜单UI
        if (createSettingsUI) CreateSettingsUI();    // 创建设置UI
        if (createCreditsUI) CreateCreditsUI();      // 创建制作人员UI
        if (createLoadingUI) CreateLoadingUI();      // 创建加载UI
        
        SetupScripts();      // 设置脚本组件
        SetupReferences();   // 设置引用关系
    }
    
    /// <summary>
    /// 创建主菜单UI
    /// 创建主菜单面板、背景、标题和按钮
    /// </summary>
    private void CreateMainMenuUI()
    {
        // 如果主画布不存在，则创建
        if (mainCanvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            mainCanvas = canvasObj.AddComponent<Canvas>();
            mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 创建主菜单面板
        GameObject mainMenuPanel = CreatePanel("MainMenuPanel", mainCanvas.transform);
        
        // 创建背景图片
        GameObject backgroundImage = CreateImage("BackgroundImage", mainMenuPanel.transform);
        RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        // 创建标题
        GameObject titleText = CreateText("TitleText", mainMenuPanel.transform, "LOOOP");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(400, 100);
        
        // 创建按钮
        CreateButton("StartGameButton", mainMenuPanel.transform, "开始游戏", new Vector2(0.5f, 0.6f));
        CreateButton("ContinueGameButton", mainMenuPanel.transform, "继续游戏", new Vector2(0.5f, 0.5f));
        CreateButton("SettingsButton", mainMenuPanel.transform, "设置", new Vector2(0.5f, 0.4f));
        CreateButton("CreditsButton", mainMenuPanel.transform, "制作人员", new Vector2(0.5f, 0.3f));
        CreateButton("ExitButton", mainMenuPanel.transform, "退出", new Vector2(0.5f, 0.2f));
    }
    
    private void CreateSettingsUI()
    {
        GameObject settingsPanel = CreatePanel("SettingsPanel", mainCanvas.transform);
        settingsPanel.SetActive(false);
        
        // Create Title
        GameObject titleText = CreateText("TitleText", settingsPanel.transform, "设置");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(200, 50);
        
        // Create Volume Sliders
        CreateSlider("MasterVolumeSlider", settingsPanel.transform, "主音量", new Vector2(0.5f, 0.8f));
        CreateSlider("MusicVolumeSlider", settingsPanel.transform, "音乐音量", new Vector2(0.5f, 0.7f));
        CreateSlider("SFXVolumeSlider", settingsPanel.transform, "音效音量", new Vector2(0.5f, 0.6f));
        
        // Create Resolution Dropdown
        CreateDropdown("ResolutionDropdown", settingsPanel.transform, "分辨率", new Vector2(0.5f, 0.5f));
        
        // Create Fullscreen Toggle
        CreateToggle("FullscreenToggle", settingsPanel.transform, "全屏", new Vector2(0.5f, 0.4f));
        
        // Create Buttons
        CreateButton("SaveButton", settingsPanel.transform, "保存", new Vector2(0.4f, 0.2f));
        CreateButton("BackButton", settingsPanel.transform, "返回", new Vector2(0.6f, 0.2f));
    }
    
    private void CreateCreditsUI()
    {
        GameObject creditsPanel = CreatePanel("CreditsPanel", mainCanvas.transform);
        creditsPanel.SetActive(false);
        
        //创建标题
        GameObject titleText = CreateText("TitleText", creditsPanel.transform, "制作人员");
        RectTransform titleRect = titleText.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.9f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(200, 50);
        
        //创建制作人员滚动视图
        GameObject scrollView = CreateScrollView("CreditsScrollView", creditsPanel.transform, new Vector2(0.5f, 0.5f));
        
        //创建返回按钮
        CreateButton("BackButton", creditsPanel.transform, "返回", new Vector2(0.5f, 0.1f));
    }
    
    private void CreateLoadingUI()
    {
        GameObject loadingPanel = CreatePanel("LoadingPanel", mainCanvas.transform);
        loadingPanel.SetActive(false);
        
        //创建进度条
        CreateProgressBar("ProgressBar", loadingPanel.transform, new Vector2(0.5f, 0.5f));
        
        //创建进度文本
        GameObject progressText = CreateText("ProgressText", loadingPanel.transform, "0%");
        RectTransform progressRect = progressText.GetComponent<RectTransform>();
        progressRect.anchorMin = new Vector2(0.5f, 0.4f);
        progressRect.anchorMax = new Vector2(0.5f, 0.45f);
        progressRect.anchoredPosition = Vector2.zero;
        progressRect.sizeDelta = new Vector2(200, 50);
        
        //创建加载文本
        GameObject loadingText = CreateText("LoadingText", loadingPanel.transform, "Loading...");
        RectTransform loadingRect = loadingText.GetComponent<RectTransform>();
        loadingRect.anchorMin = new Vector2(0.5f, 0.6f);
        loadingRect.anchorMax = new Vector2(0.5f, 0.65f);
        loadingRect.anchoredPosition = Vector2.zero;
        loadingRect.sizeDelta = new Vector2(200, 50);
    }
    
    private GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        return panel;
    }
    
    private GameObject CreateImage(string name, Transform parent)
    {
        GameObject imageObj = new GameObject(name);
        imageObj.transform.SetParent(parent, false);
        Image image = imageObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        return imageObj;
    }
    
    private GameObject CreateText(string name, Transform parent, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        textComponent.fontSize = 24;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        return textObj;
    }
    
    private GameObject CreateButton(string name, Transform parent, string text, Vector2 anchorPosition)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject textObj = CreateText("Text", buttonObj.transform, text);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = anchorPosition - new Vector2(0.1f, 0.025f);
        buttonRect.anchorMax = anchorPosition + new Vector2(0.1f, 0.025f);
        buttonRect.anchoredPosition = Vector2.zero;
        
        return buttonObj;
    }
    
    private GameObject CreateSlider(string name, Transform parent, string label, Vector2 anchorPosition)
    {
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(parent, false);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        Image sliderImage = sliderObj.AddComponent<Image>();
        sliderImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        //创建标签
        GameObject labelObj = CreateText("Label", sliderObj.transform, label);
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0.3f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.sizeDelta = new Vector2(100, 30);
        
        RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
        sliderRect.anchorMin = anchorPosition - new Vector2(0.15f, 0.025f);
        sliderRect.anchorMax = anchorPosition + new Vector2(0.15f, 0.025f);
        sliderRect.anchoredPosition = Vector2.zero;
        
        return sliderObj;
    }
    
    private GameObject CreateDropdown(string name, Transform parent, string label, Vector2 anchorPosition)
    {
        GameObject dropdownObj = new GameObject(name);
        dropdownObj.transform.SetParent(parent, false);
        
        Dropdown dropdown = dropdownObj.AddComponent<Dropdown>();
        Image dropdownImage = dropdownObj.AddComponent<Image>();
        dropdownImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        //创建标签
        GameObject labelObj = CreateText("Label", dropdownObj.transform, label);
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0.3f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.sizeDelta = new Vector2(100, 30);
        
        RectTransform dropdownRect = dropdownObj.GetComponent<RectTransform>();
        dropdownRect.anchorMin = anchorPosition - new Vector2(0.15f, 0.025f);
        dropdownRect.anchorMax = anchorPosition + new Vector2(0.15f, 0.025f);
        dropdownRect.anchoredPosition = Vector2.zero;
        
        return dropdownObj;
    }
    
    private GameObject CreateToggle(string name, Transform parent, string label, Vector2 anchorPosition)
    {
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(parent, false);
        
        Toggle toggle = toggleObj.AddComponent<Toggle>();
        Image toggleImage = toggleObj.AddComponent<Image>();
        toggleImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        //创建标签
        GameObject labelObj = CreateText("Label", toggleObj.transform, label);
        RectTransform labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, 0.5f);
        labelRect.anchorMax = new Vector2(0.9f, 0.5f);
        labelRect.anchoredPosition = Vector2.zero;
        labelRect.sizeDelta = new Vector2(0, 30);
        
        RectTransform toggleRect = toggleObj.GetComponent<RectTransform>();
        toggleRect.anchorMin = anchorPosition - new Vector2(0.15f, 0.025f);
        toggleRect.anchorMax = anchorPosition + new Vector2(0.15f, 0.025f);
        toggleRect.anchoredPosition = Vector2.zero;
        
        return toggleObj;
    }
    
    private GameObject CreateScrollView(string name, Transform parent, Vector2 anchorPosition)
    {
        GameObject scrollViewObj = new GameObject(name);
        scrollViewObj.transform.SetParent(parent, false);
        
        ScrollRect scrollRect = scrollViewObj.AddComponent<ScrollRect>();
        Image scrollImage = scrollViewObj.AddComponent<Image>();
        scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        //创建内容
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(scrollViewObj.transform, false);
        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;
        
        //创建制作人员文本
        GameObject creditsText = CreateText("CreditsText", contentObj.transform, "制作人员信息将在这里显示...");
        RectTransform creditsRect = creditsText.GetComponent<RectTransform>();
        creditsRect.anchorMin = Vector2.zero;
        creditsRect.anchorMax = new Vector2(1f, 1f);
        creditsRect.offsetMin = new Vector2(20, 20);
        creditsRect.offsetMax = new Vector2(-20, -20);
        
        scrollRect.content = contentRect;
        
        RectTransform scrollViewRect = scrollViewObj.GetComponent<RectTransform>();
        scrollViewRect.anchorMin = anchorPosition - new Vector2(0.4f, 0.3f);
        scrollViewRect.anchorMax = anchorPosition + new Vector2(0.4f, 0.3f);
        scrollViewRect.anchoredPosition = Vector2.zero;
        
        return scrollViewObj;
    }
    
    private GameObject CreateProgressBar(string name, Transform parent, Vector2 anchorPosition)
    {
        GameObject progressBarObj = new GameObject(name);
        progressBarObj.transform.SetParent(parent, false);
        
        Slider progressSlider = progressBarObj.AddComponent<Slider>();
        Image progressImage = progressBarObj.AddComponent<Image>();
        progressImage.color = new Color(0.3f, 0.3f, 0.3f, 1f);
        
        RectTransform progressRect = progressBarObj.GetComponent<RectTransform>();
        progressRect.anchorMin = anchorPosition - new Vector2(0.3f, 0.025f);
        progressRect.anchorMax = anchorPosition + new Vector2(0.3f, 0.025f);
        progressRect.anchoredPosition = Vector2.zero;
        
        return progressBarObj;
    }
    
    private void SetupScripts()
    {
        if (mainMenuManager == null)
        {
            mainMenuManager = mainCanvas.gameObject.AddComponent<MainMenuManager>();
        }
        
        if (uiManager == null)
        {
            uiManager = mainCanvas.gameObject.AddComponent<UIManager>();
        }
        
        GameObject settingsPanel = mainCanvas.transform.Find("SettingsPanel")?.gameObject;
        if (settingsPanel != null && settingsManager == null)
        {
            settingsManager = settingsPanel.AddComponent<SettingsManager>();
        }
        
        GameObject creditsPanel = mainCanvas.transform.Find("CreditsPanel")?.gameObject;
        if (creditsPanel != null && creditsManager == null)
        {
            creditsManager = creditsPanel.AddComponent<CreditsManager>();
        }
    }
    
    private void SetupReferences()
    {
        if (mainMenuManager != null)
        {
            mainMenuManager.mainMenuPanel = mainCanvas.transform.Find("MainMenuPanel")?.gameObject;
            mainMenuManager.settingsPanel = mainCanvas.transform.Find("SettingsPanel")?.gameObject;
            mainMenuManager.creditsPanel = mainCanvas.transform.Find("CreditsPanel")?.gameObject;
            mainMenuManager.backgroundImage = mainCanvas.transform.Find("MainMenuPanel/BackgroundImage")?.GetComponent<Image>();
            
            mainMenuManager.startGameButton = mainCanvas.transform.Find("MainMenuPanel/StartGameButton")?.GetComponent<Button>();
            mainMenuManager.continueGameButton = mainCanvas.transform.Find("MainMenuPanel/ContinueGameButton")?.GetComponent<Button>();
            mainMenuManager.settingsButton = mainCanvas.transform.Find("MainMenuPanel/SettingsButton")?.GetComponent<Button>();
            mainMenuManager.creditsButton = mainCanvas.transform.Find("MainMenuPanel/CreditsButton")?.GetComponent<Button>();
            mainMenuManager.exitButton = mainCanvas.transform.Find("MainMenuPanel/ExitButton")?.GetComponent<Button>();
        }
        
        if (uiManager != null)
        {
            uiManager.mainCanvas = mainCanvas;
            uiManager.canvasScaler = mainCanvas.GetComponent<CanvasScaler>();
        }
    }
    #endif
} 