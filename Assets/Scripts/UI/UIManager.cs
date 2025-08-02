using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI管理器
/// 负责管理UI的缩放、按钮动画和面板显示
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI组件")]
    public Canvas mainCanvas;           // 主画布
    public CanvasScaler canvasScaler;   // 画布缩放器
    
    [Header("按钮动画设置")]
    public float buttonHoverScale = 1.1f;    // 鼠标悬停时的缩放比例
    public float buttonClickScale = 0.95f;   // 点击时的缩放比例
    public float animationDuration = 0.1f;   // 动画持续时间
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void Start()
    {
        SetupCanvas();           // 设置画布缩放
        SetupButtonAnimations(); // 设置按钮动画
    }
    
    /// <summary>
    /// 设置画布缩放器
    /// 配置画布以适应不同屏幕尺寸
    /// </summary>
    private void SetupCanvas()
    {
        if (canvasScaler != null)
        {
            // 设置画布缩放模式以适应不同屏幕尺寸
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080); // 参考分辨率
            canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight; // 屏幕匹配模式
            canvasScaler.matchWidthOrHeight = 0.5f; // 宽高匹配比例
        }
    }
    
    /// <summary>
    /// 设置按钮动画
    /// 为所有按钮添加动画组件
    /// </summary>
    private void SetupButtonAnimations()
    {
        // 查找所有按钮并添加动画组件
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (Button button in buttons)
        {
            AddButtonAnimation(button); // 为每个按钮添加动画
        }
    }
    
    /// <summary>
    /// 为按钮添加动画组件
    /// </summary>
    /// <param name="button">要添加动画的按钮</param>
    private void AddButtonAnimation(Button button)
    {
        // 为按钮添加悬停和点击动画
        ButtonAnimation anim = button.gameObject.GetComponent<ButtonAnimation>();
        if (anim == null)
        {
            anim = button.gameObject.AddComponent<ButtonAnimation>(); // 添加动画组件
        }
        
        // 设置动画参数
        anim.hoverScale = buttonHoverScale;      // 悬停缩放
        anim.clickScale = buttonClickScale;      // 点击缩放
        anim.animationDuration = animationDuration; // 动画持续时间
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    /// <param name="panel">要显示的面板</param>
    public void ShowPanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(true); // 激活面板
        }
    }
    
    /// <summary>
    /// 隐藏面板
    /// </summary>
    /// <param name="panel">要隐藏的面板</param>
    public void HidePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(false); // 停用面板
        }
    }
    
    /// <summary>
    /// 切换面板显示状态
    /// </summary>
    /// <param name="panel">要切换的面板</param>
    public void TogglePanel(GameObject panel)
    {
        if (panel != null)
        {
            panel.SetActive(!panel.activeSelf); // 切换激活状态
        }
    }
}

/// <summary>
/// 按钮动画组件
/// 为按钮提供悬停和点击动画效果
/// </summary>
public class ButtonAnimation : MonoBehaviour
{
    public float hoverScale = 1.1f;        // 悬停时的缩放比例
    public float clickScale = 0.95f;       // 点击时的缩放比例
    public float animationDuration = 0.1f; // 动画持续时间
    
    private Vector3 originalScale;         // 原始缩放值
    private Button button;                 // 按钮组件
    
    /// <summary>
    /// 初始化时调用
    /// </summary>
    private void Start()
    {
        originalScale = transform.localScale; // 保存原始缩放值
        button = GetComponent<Button>();      // 获取按钮组件
        
        if (button != null)
        {
            button.onClick.AddListener(OnClick); // 添加点击监听
        }
    }
    
    /// <summary>
    /// 鼠标进入时触发
    /// </summary>
    private void OnMouseEnter()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = originalScale * hoverScale; // 应用悬停缩放
        }
    }
    
    /// <summary>
    /// 鼠标离开时触发
    /// </summary>
    private void OnMouseExit()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = originalScale; // 恢复原始缩放
        }
    }
    
    /// <summary>
    /// 点击时触发
    /// </summary>
    private void OnClick()
    {
        if (button != null && button.interactable)
        {
            transform.localScale = originalScale * clickScale; // 应用点击缩放
            Invoke(nameof(ResetScale), animationDuration);     // 延迟恢复原始缩放
        }
    }
    
    /// <summary>
    /// 重置缩放到原始值
    /// </summary>
    private void ResetScale()
    {
        transform.localScale = originalScale; // 恢复原始缩放
    }
} 