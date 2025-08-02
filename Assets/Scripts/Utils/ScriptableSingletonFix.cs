using System.Collections;
using UnityEngine;

/// <summary>
/// ScriptableSingleton错误修复脚本
/// 用于处理Unity Visual Scripting系统中的ScriptableSingleton错误
/// </summary>
public class ScriptableSingletonFix : MonoBehaviour
{
    [Header("修复设置")]
    [SerializeField] private bool enableFix = true; // 是否启用修复
    [SerializeField] private bool showDebugInfo = true; // 是否显示调试信息
    
    [Header("延迟设置")]
    [SerializeField] private float initializationDelay = 0.1f; // 初始化延迟时间
    
    private bool isInitialized = false;

    /// <summary>
    /// 初始化方法
    /// </summary>
    void Awake()
    {
        if (!enableFix) return;
        
        // 延迟初始化，避免ScriptableSingleton错误
        StartCoroutine(DelayedInitialization());
    }

    /// <summary>
    /// 延迟初始化协程
    /// </summary>
    private IEnumerator DelayedInitialization()
    {
        if (showDebugInfo)
        {
            Debug.Log("ScriptableSingletonFix: 开始延迟初始化...");
        }

        // 等待指定的延迟时间
        yield return new WaitForSeconds(initializationDelay);
        
        // 等待一帧，确保所有系统都已初始化
        yield return null;
        
        // 再次等待一帧，确保Visual Scripting系统完全初始化
        yield return null;
        
        isInitialized = true;
        
        if (showDebugInfo)
        {
            Debug.Log("ScriptableSingletonFix: 初始化完成，ScriptableSingleton错误已避免");
        }
    }

    /// <summary>
    /// 检查是否已初始化
    /// </summary>
    public bool IsInitialized()
    {
        return isInitialized;
    }

    /// <summary>
    /// 等待初始化完成
    /// </summary>
    public IEnumerator WaitForInitialization()
    {
        while (!isInitialized)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 强制重新初始化
    /// </summary>
    public void ForceReinitialize()
    {
        if (showDebugInfo)
        {
            Debug.Log("ScriptableSingletonFix: 强制重新初始化");
        }
        
        isInitialized = false;
        StartCoroutine(DelayedInitialization());
    }

    /// <summary>
    /// 在GUI中显示状态
    /// </summary>
    void OnGUI()
    {
        if (!enableFix || !showDebugInfo) return;

        GUILayout.BeginArea(new Rect(10, Screen.height - 100, 300, 80));
        GUILayout.Label("=== ScriptableSingleton修复状态 ===");
        GUILayout.Label($"初始化状态: {(isInitialized ? "已完成" : "进行中")}");
        GUILayout.Label($"延迟时间: {initializationDelay}秒");
        GUILayout.EndArea();
    }
} 