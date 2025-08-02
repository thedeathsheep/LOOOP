using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简单模块测试器 - 提供直观的测试界面
/// </summary>
public class SimpleModuleTester : MonoBehaviour
{
    [Header("测试组件")]
    public ActionModule actionModule;
    
    [Header("测试设置")]
    public bool showTestUI = true;          // 是否显示测试UI
    public bool autoStopAfterTest = true;   // 测试后自动停止
    
    [Header("当前状态")]
    public string currentModuleName = "";
    public bool isTesting = false;
    public float testStartTime;
    
    // 常用测试模块列表
    private string[] commonModules = {
        "立定跳跃", "左跳", "右跳",
        "左冲刺", "右冲刺", "上冲刺", "下冲刺",
        "左走", "右走", "蹲下"
    };
    
    void Start()
    {
        if (actionModule == null)
        {
            actionModule = GetComponent<ActionModule>();
        }
        
        if (actionModule == null)
        {
            Debug.LogError("未找到ActionModule组件！");
        }
    }
    
    void Update()
    {
        // 键盘快捷键测试
        HandleKeyboardShortcuts();
        
        // 更新测试状态
        UpdateTestStatus();
    }
    
    /// <summary>
    /// 处理键盘快捷键
    /// </summary>
    void HandleKeyboardShortcuts()
    {
        // 数字键1-0快速测试常用模块
        for (int i = 0; i < Mathf.Min(commonModules.Length, 10); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                TestModule(commonModules[i]);
            }
        }
        
        // 字母键测试特定动作
        if (Input.GetKeyDown(KeyCode.J)) TestModule("立定跳跃");
        if (Input.GetKeyDown(KeyCode.L)) TestModule("左跳");
        if (Input.GetKeyDown(KeyCode.R)) TestModule("右跳");
        if (Input.GetKeyDown(KeyCode.D)) TestModule("右冲刺");
        if (Input.GetKeyDown(KeyCode.A)) TestModule("左冲刺");
        if (Input.GetKeyDown(KeyCode.W)) TestModule("上冲刺");
        if (Input.GetKeyDown(KeyCode.S)) TestModule("下冲刺");
        if (Input.GetKeyDown(KeyCode.C)) TestModule("蹲下");
        
        // 组合动作测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestJumpDashCombination();
        }
        
        // 停止当前测试
        if (Input.GetKeyDown(KeyCode.X))
        {
            StopCurrentTest();
        }
        
        // 清除所有测试
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearAllTests();
        }
    }
    
    /// <summary>
    /// 测试指定模块
    /// </summary>
    public void TestModule(string moduleName)
    {
        if (actionModule == null)
        {
            Debug.LogError("ActionModule未找到！");
            return;
        }
        
        var module = actionModule.FindModuleByName(moduleName);
        if (module == null)
        {
            Debug.LogError($"未找到模块: {moduleName}");
            return;
        }
        
        // 停止当前测试
        if (isTesting)
        {
            actionModule.StopModule();
        }
        
        // 开始新测试
        currentModuleName = moduleName;
        isTesting = true;
        testStartTime = Time.time;
        
        Debug.Log($"开始测试模块: {moduleName}");
        actionModule.ExecuteModule(module);
        
        // 如果设置了自动停止，在模块执行完成后停止
        if (autoStopAfterTest)
        {
            StartCoroutine(AutoStopAfterDuration(module.totalDuration + 0.5f));
        }
    }
    
    /// <summary>
    /// 测试跳跃+冲刺组合
    /// </summary>
    public void TestJumpDashCombination()
    {
        if (actionModule == null) return;
        
        var actions = new List<string> { "立定跳跃", "右冲刺" };
        currentModuleName = "跳跃+冲刺组合";
        isTesting = true;
        testStartTime = Time.time;
        
        Debug.Log("开始测试跳跃+冲刺组合");
        actionModule.ExecuteActionSequence(actions, 0.3f);
        
        if (autoStopAfterTest)
        {
            StartCoroutine(AutoStopAfterDuration(2f));
        }
    }
    
    /// <summary>
    /// 测试连续冲刺
    /// </summary>
    public void TestContinuousDash()
    {
        if (actionModule == null) return;
        
        var actions = new List<string> { "右冲刺", "右冲刺", "右冲刺" };
        currentModuleName = "连续冲刺";
        isTesting = true;
        testStartTime = Time.time;
        
        Debug.Log("开始测试连续冲刺");
        actionModule.ExecuteActionSequence(actions, 0.1f);
        
        if (autoStopAfterTest)
        {
            StartCoroutine(AutoStopAfterDuration(1.5f));
        }
    }
    
    /// <summary>
    /// 测试复杂组合
    /// </summary>
    public void TestComplexCombination()
    {
        if (actionModule == null) return;
        
        var actions = new List<string> { "右走", "右跳", "右冲刺", "蹲下" };
        currentModuleName = "复杂组合";
        isTesting = true;
        testStartTime = Time.time;
        
        Debug.Log("开始测试复杂组合");
        actionModule.ExecuteActionSequence(actions, 0.2f);
        
        if (autoStopAfterTest)
        {
            StartCoroutine(AutoStopAfterDuration(3f));
        }
    }
    
    /// <summary>
    /// 停止当前测试
    /// </summary>
    public void StopCurrentTest()
    {
        if (actionModule != null)
        {
            actionModule.StopModule();
        }
        
        isTesting = false;
        currentModuleName = "";
        Debug.Log("测试已停止");
    }
    
    /// <summary>
    /// 清除所有测试
    /// </summary>
    public void ClearAllTests()
    {
        StopCurrentTest();
        Debug.Log("所有测试已清除");
    }
    
    /// <summary>
    /// 更新测试状态
    /// </summary>
    void UpdateTestStatus()
    {
        if (isTesting && actionModule != null)
        {
            // 检查是否还在执行
            if (!actionModule.isExecuting)
            {
                isTesting = false;
                currentModuleName = "";
            }
        }
    }
    
    /// <summary>
    /// 自动停止协程
    /// </summary>
    IEnumerator AutoStopAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        
        if (isTesting)
        {
            StopCurrentTest();
        }
    }
    
    /// <summary>
    /// 在Scene视图中显示测试UI
    /// </summary>
    void OnGUI()
    {
        if (!showTestUI) return;
        
        // 显示当前测试状态
        if (isTesting)
        {
            float elapsedTime = Time.time - testStartTime;
            GUI.color = Color.yellow;
            GUI.Label(new Rect(10, 100, 300, 20), $"正在测试: {currentModuleName}");
            GUI.Label(new Rect(10, 120, 300, 20), $"测试时间: {elapsedTime:F2}s");
        }
        else
        {
            GUI.color = Color.white;
            GUI.Label(new Rect(10, 100, 300, 20), "当前无测试运行");
        }
        
        GUI.color = Color.white;
        
        // 显示快捷键说明
        int yPos = 150;
        GUI.Label(new Rect(10, yPos, 400, 20), "=== 快捷键说明 ===");
        yPos += 25;
        
        GUI.Label(new Rect(10, yPos, 400, 20), "数字键1-0: 快速测试常用模块");
        yPos += 20;
        GUI.Label(new Rect(10, yPos, 400, 20), "J: 立定跳跃  L: 左跳  R: 右跳");
        yPos += 20;
        GUI.Label(new Rect(10, yPos, 400, 20), "A: 左冲刺  D: 右冲刺  W: 上冲刺  S: 下冲刺");
        yPos += 20;
        GUI.Label(new Rect(10, yPos, 400, 20), "C: 蹲下  空格: 跳跃+冲刺组合");
        yPos += 20;
        GUI.Label(new Rect(10, yPos, 400, 20), "X: 停止测试  ESC: 清除所有测试");
        
        // 显示可用模块列表
        yPos += 30;
        GUI.Label(new Rect(10, yPos, 400, 20), "=== 可用模块 ===");
        yPos += 25;
        
        if (actionModule != null)
        {
            var modules = actionModule.GetAvailableModules();
            foreach (var module in modules)
            {
                GUI.Label(new Rect(10, yPos, 400, 20), 
                    $"{module.moduleName}: {module.description}");
                yPos += 20;
            }
        }
    }
    
    /// <summary>
    /// 右键菜单测试方法
    /// </summary>
    [ContextMenu("测试立定跳跃")]
    void TestJump()
    {
        TestModule("立定跳跃");
    }
    
    [ContextMenu("测试左跳")]
    void TestLeftJump()
    {
        TestModule("左跳");
    }
    
    [ContextMenu("测试右跳")]
    void TestRightJump()
    {
        TestModule("右跳");
    }
    
    [ContextMenu("测试右冲刺")]
    void TestRightDash()
    {
        TestModule("右冲刺");
    }
    
    [ContextMenu("测试左冲刺")]
    void TestLeftDash()
    {
        TestModule("左冲刺");
    }
    
    [ContextMenu("测试蹲下")]
    void TestCrouch()
    {
        TestModule("蹲下");
    }
    
    [ContextMenu("测试跳跃+冲刺组合")]
    void TestJumpDash()
    {
        TestJumpDashCombination();
    }
    
    [ContextMenu("停止当前测试")]
    void StopTest()
    {
        StopCurrentTest();
    }
} 