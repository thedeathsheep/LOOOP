using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 主菜单管理器
/// 负责管理游戏主菜单的所有功能和界面切换
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject mainMenuPanel;    // 主菜单面板
    public GameObject settingsPanel;    // 设置面板
    public GameObject creditsPanel;     // 制作人员面板
    
    [Header("主菜单按钮")]
    public Button startGameButton;      // 开始游戏按钮
    public Button continueGameButton;   // 继续游戏按钮
    public Button settingsButton;       // 设置按钮
    public Button creditsButton;        // 制作人员按钮
    public Button exitButton;           // 退出游戏按钮
    
    [Header("背景")]
    public Image backgroundImage;       // 背景图片
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void Start()
    {
        SetupButtons();     // 设置按钮事件
        ShowMainMenu();     // 显示主菜单
        
        // 检查是否有存档，如果没有则禁用继续游戏按钮
        continueGameButton.interactable = PlayerPrefs.HasKey("SavedGame");
    }
    
    /// <summary>
    /// 设置所有按钮的点击事件
    /// </summary>
    private void SetupButtons()
    {
        startGameButton.onClick.AddListener(StartNewGame);    // 开始新游戏
        continueGameButton.onClick.AddListener(ContinueGame); // 继续游戏
        settingsButton.onClick.AddListener(ShowSettings);     // 显示设置
        creditsButton.onClick.AddListener(ShowCredits);       // 显示制作人员
        exitButton.onClick.AddListener(ExitGame);             // 退出游戏
    }
    
    /// <summary>
    /// 开始新游戏
    /// 清除存档数据并加载第一关
    /// </summary>
    public void StartNewGame()
    {
        // 清除存档数据
        PlayerPrefs.DeleteKey("SavedGame");
        PlayerPrefs.Save();
        
        // 加载第一关场景
        SceneManager.LoadScene("Level1");
    }
    
    /// <summary>
    /// 继续游戏
    /// 加载上次保存的游戏进度
    /// </summary>
    public void ContinueGame()
    {
        if (PlayerPrefs.HasKey("SavedGame"))
        {
            // 加载保存的游戏场景
            string savedScene = PlayerPrefs.GetString("SavedGame");
            SceneManager.LoadScene(savedScene);
        }
    }
    
    /// <summary>
    /// 显示设置面板
    /// 隐藏主菜单，显示设置界面
    /// </summary>
    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);  // 隐藏主菜单
        settingsPanel.SetActive(true);   // 显示设置面板
    }
    
    /// <summary>
    /// 显示制作人员面板
    /// 隐藏主菜单，显示制作人员界面
    /// </summary>
    public void ShowCredits()
    {
        mainMenuPanel.SetActive(false);  // 隐藏主菜单
        creditsPanel.SetActive(true);    // 显示制作人员面板
    }
    
    /// <summary>
    /// 显示主菜单
    /// 隐藏其他面板，显示主菜单界面
    /// </summary>
    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);   // 显示主菜单
        settingsPanel.SetActive(false);  // 隐藏设置面板
        creditsPanel.SetActive(false);   // 隐藏制作人员面板
    }
    
    /// <summary>
    /// 退出游戏
    /// 在编辑器中停止播放，在构建版本中退出应用
    /// </summary>
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // 编辑器模式下停止播放
        #else
            Application.Quit(); // 构建版本中退出应用
        #endif
    }
} 