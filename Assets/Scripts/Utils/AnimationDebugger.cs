using UnityEngine;

/// <summary>
/// 动画调试器 - 用于诊断动画系统问题
/// </summary>
public class AnimationDebugger : MonoBehaviour
{
    [Header("调试设置")]
    public bool showAnimationDebug = true;
    public bool showAnimatorInfo = true;
    
    private Animator animator;
    private UpdateAnimation updateAnimation;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        updateAnimation = GetComponent<UpdateAnimation>();
    }
    
    void Update()
    {
        if (!showAnimationDebug) return;
        
        // 显示动画控制器信息
        if (showAnimatorInfo && animator != null)
        {
            if (Input.GetKeyDown("i"))
            {
                Debug.Log("=== 动画控制器信息 ===");
                Debug.Log($"动画控制器: {animator.runtimeAnimatorController?.name ?? "null"}");
                Debug.Log($"当前状态: {animator.GetCurrentAnimatorStateInfo(0).IsName("")}");
                Debug.Log($"动画参数数量: {animator.parameters.Length}");
                
                foreach (var param in animator.parameters)
                {
                    Debug.Log($"参数: {param.name}, 类型: {param.type}, 默认值: {param.defaultInt}");
                }
            }
        }
    }
    
    void OnGUI()
    {
        if (!showAnimationDebug) return;
        
        GUILayout.BeginArea(new Rect(10, 220, 400, 200));
        GUILayout.Label("=== 动画调试器 ===");
        
        if (animator != null)
        {
            GUILayout.Label($"动画控制器: {animator.runtimeAnimatorController?.name ?? "null"}");
            GUILayout.Label($"参数数量: {animator.parameters.Length}");
            
            if (animator.runtimeAnimatorController != null)
            {
                GUILayout.Label("参数列表:");
                foreach (var param in animator.parameters)
                {
                    GUILayout.Label($"- {param.name} ({param.type})");
                }
            }
        }
        else
        {
            GUILayout.Label("未找到 Animator 组件");
        }
        
        GUILayout.Label("按键:");
        GUILayout.Label("- I: 显示动画控制器信息");
        
        GUILayout.EndArea();
    }
} 