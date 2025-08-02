using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 制作人员管理器
/// 负责显示游戏制作人员信息和管理制作人员界面的交互
/// </summary>
public class CreditsManager : MonoBehaviour
{
    [Header("UI组件")]
    public Button backButton;           // 返回按钮
    public ScrollRect creditsScrollRect; // 制作人员滚动视图
    public Text creditsText;            // 制作人员文本显示
    
    [Header("制作人员内容")]
    [TextArea(10, 20)]
    public string creditsContent = @"LOOOP Game Credits

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
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void Start()
    {
        SetupCredits();  // 设置制作人员内容
        SetupButtons();  // 设置按钮事件
    }
    
    /// <summary>
    /// 设置制作人员内容到UI文本组件
    /// </summary>
    private void SetupCredits()
    {
        if (creditsText != null)
        {
            creditsText.text = creditsContent; // 将制作人员内容设置到文本组件
        }
    }
    
    /// <summary>
    /// 设置按钮点击事件
    /// </summary>
    private void SetupButtons()
    {
        backButton.onClick.AddListener(BackToMainMenu); // 为返回按钮添加点击事件
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
} 