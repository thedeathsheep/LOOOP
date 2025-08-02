using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ActionModule测试器 - 用于测试不同的动作模块功能
/// </summary>
public class ModuleTester : MonoBehaviour
{
    [Header("测试组件")]
    public ActionModule actionModule;
    
    [Header("测试设置")]
    public bool autoTest = false;           // 是否自动测试
    public float testInterval = 3f;         // 测试间隔时间
    public int currentTestIndex = 0;        // 当前测试索引
    
    [Header("测试结果")]
    public List<TestResult> testResults = new List<TestResult>();
    
    [System.Serializable]
    public class TestResult
    {
        public string testName;
        public bool passed;
        public string message;
        public float executionTime;
        
        public TestResult(string name, bool pass, string msg, float time)
        {
            testName = name;
            passed = pass;
            message = msg;
            executionTime = time;
        }
    }
    
    // 测试用例列表
    private List<TestCase> testCases = new List<TestCase>();
    
    [System.Serializable]
    public class TestCase
    {
        public string name;
        public string description;
        public System.Action<ModuleTester> testAction;
        
        public TestCase(string testName, string desc, System.Action<ModuleTester> action)
        {
            name = testName;
            description = desc;
            testAction = action;
        }
    }
    
    void Start()
    {
        // 获取ActionModule组件
        if (actionModule == null)
        {
            actionModule = GetComponent<ActionModule>();
        }
        
        if (actionModule == null)
        {
            Debug.LogError("未找到ActionModule组件！");
            return;
        }
        
        // 初始化测试用例
        InitializeTestCases();
        
        if (autoTest)
        {
            StartCoroutine(AutoTestCoroutine());
        }
    }
    
    void Update()
    {
        // 键盘快捷键测试
        HandleKeyboardTests();
    }
    
    /// <summary>
    /// 初始化测试用例
    /// </summary>
    void InitializeTestCases()
    {
        testCases.Clear();
        
        // 基础动作测试
        testCases.Add(new TestCase("基础跳跃测试", "测试立定跳跃功能", TestBasicJump));
        testCases.Add(new TestCase("方向跳跃测试", "测试左右跳跃功能", TestDirectionalJump));
        testCases.Add(new TestCase("冲刺测试", "测试四个方向冲刺", TestDash));
        testCases.Add(new TestCase("行走测试", "测试左右行走", TestWalk));
        testCases.Add(new TestCase("蹲下测试", "测试蹲下功能", TestCrouch));
        
        // 组合动作测试
        testCases.Add(new TestCase("跳跃+冲刺组合", "测试跳跃后冲刺的组合", TestJumpDashCombination));
        testCases.Add(new TestCase("连续冲刺", "测试连续冲刺动作", TestContinuousDash));
        testCases.Add(new TestCase("复杂组合", "测试复杂的动作组合", TestComplexCombination));
        
        // 性能测试
        testCases.Add(new TestCase("快速切换测试", "测试快速切换不同动作", TestRapidSwitching));
        testCases.Add(new TestCase("长时间执行", "测试长时间执行动作", TestLongExecution));
        
        Debug.Log($"初始化了 {testCases.Count} 个测试用例");
    }
    
    /// <summary>
    /// 处理键盘测试快捷键
    /// </summary>
    void HandleKeyboardTests()
    {
        // 数字键1-9快速测试
        for (int i = 0; i < Mathf.Min(testCases.Count, 9); i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                RunTest(i);
            }
        }
        
        // 空格键运行当前测试
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RunTest(currentTestIndex);
        }
        
        // 左右箭头切换测试
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTestIndex = (currentTestIndex - 1 + testCases.Count) % testCases.Count;
            Debug.Log($"切换到测试: {testCases[currentTestIndex].name}");
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentTestIndex = (currentTestIndex + 1) % testCases.Count;
            Debug.Log($"切换到测试: {testCases[currentTestIndex].name}");
        }
        
        // R键重新运行当前测试
        if (Input.GetKeyDown(KeyCode.R))
        {
            RunTest(currentTestIndex);
        }
        
        // T键运行所有测试
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(RunAllTests());
        }
        
        // C键清除测试结果
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearTestResults();
        }
    }
    
    /// <summary>
    /// 运行指定索引的测试
    /// </summary>
    public void RunTest(int testIndex)
    {
        if (testIndex < 0 || testIndex >= testCases.Count)
        {
            Debug.LogError($"测试索引超出范围: {testIndex}");
            return;
        }
        
        var testCase = testCases[testIndex];
        Debug.Log($"开始运行测试: {testCase.name} - {testCase.description}");
        
        float startTime = Time.time;
        bool testPassed = false;
        string testMessage = "";
        
        try
        {
            testCase.testAction(this);
            testPassed = true;
            testMessage = "测试执行成功";
        }
        catch (System.Exception e)
        {
            testPassed = false;
            testMessage = $"测试执行失败: {e.Message}";
        }
        
        float executionTime = Time.time - startTime;
        
        // 记录测试结果
        var result = new TestResult(testCase.name, testPassed, testMessage, executionTime);
        testResults.Add(result);
        
        Debug.Log($"测试完成: {testCase.name} - {(testPassed ? "通过" : "失败")} - 耗时: {executionTime:F3}s");
    }
    
    /// <summary>
    /// 自动测试协程
    /// </summary>
    IEnumerator AutoTestCoroutine()
    {
        Debug.Log("开始自动测试...");
        
        for (int i = 0; i < testCases.Count; i++)
        {
            RunTest(i);
            yield return new WaitForSeconds(testInterval);
        }
        
        Debug.Log("自动测试完成！");
        PrintTestSummary();
    }
    
    /// <summary>
    /// 运行所有测试
    /// </summary>
    IEnumerator RunAllTests()
    {
        Debug.Log("开始运行所有测试...");
        
        for (int i = 0; i < testCases.Count; i++)
        {
            RunTest(i);
            yield return new WaitForSeconds(1f); // 短暂等待
        }
        
        Debug.Log("所有测试完成！");
        PrintTestSummary();
    }
    
    /// <summary>
    /// 打印测试总结
    /// </summary>
    void PrintTestSummary()
    {
        int passed = 0;
        int failed = 0;
        float totalTime = 0f;
        
        foreach (var result in testResults)
        {
            if (result.passed) passed++;
            else failed++;
            totalTime += result.executionTime;
        }
        
        Debug.Log($"=== 测试总结 ===");
        Debug.Log($"总测试数: {testResults.Count}");
        Debug.Log($"通过: {passed}");
        Debug.Log($"失败: {failed}");
        Debug.Log($"总耗时: {totalTime:F3}s");
        Debug.Log($"平均耗时: {totalTime / testResults.Count:F3}s");
    }
    
    /// <summary>
    /// 清除测试结果
    /// </summary>
    void ClearTestResults()
    {
        testResults.Clear();
        Debug.Log("测试结果已清除");
    }
    
    // ========== 具体测试方法 ==========
    
    /// <summary>
    /// 测试基础跳跃
    /// </summary>
    void TestBasicJump(ModuleTester tester)
    {
        var module = actionModule.FindModuleByName("立定跳跃");
        if (module == null)
        {
            throw new System.Exception("未找到立定跳跃模块");
        }
        
        actionModule.ExecuteModule(module);
    }
    
    /// <summary>
    /// 测试方向跳跃
    /// </summary>
    void TestDirectionalJump(ModuleTester tester)
    {
        var leftJump = actionModule.FindModuleByName("左跳");
        var rightJump = actionModule.FindModuleByName("右跳");
        
        if (leftJump == null || rightJump == null)
        {
            throw new System.Exception("未找到方向跳跃模块");
        }
        
        // 先左跳，再右跳
        actionModule.ExecuteModule(leftJump);
        StartCoroutine(DelayedExecute(rightJump, 1f));
    }
    
    /// <summary>
    /// 测试冲刺
    /// </summary>
    void TestDash(ModuleTester tester)
    {
        var directions = new[] { "左冲刺", "右冲刺", "上冲刺", "下冲刺" };
        
        foreach (var direction in directions)
        {
            var module = actionModule.FindModuleByName(direction);
            if (module == null)
            {
                throw new System.Exception($"未找到{direction}模块");
            }
        }
        
        // 执行四个方向的冲刺
        StartCoroutine(ExecuteSequentialModules(directions, 0.5f));
    }
    
    /// <summary>
    /// 测试行走
    /// </summary>
    void TestWalk(ModuleTester tester)
    {
        var leftWalk = actionModule.FindModuleByName("左走");
        var rightWalk = actionModule.FindModuleByName("右走");
        
        if (leftWalk == null || rightWalk == null)
        {
            throw new System.Exception("未找到行走模块");
        }
        
        // 先左走，再右走
        actionModule.ExecuteModule(leftWalk);
        StartCoroutine(DelayedExecute(rightWalk, 2f));
    }
    
    /// <summary>
    /// 测试蹲下
    /// </summary>
    void TestCrouch(ModuleTester tester)
    {
        var crouch = actionModule.FindModuleByName("蹲下");
        if (crouch == null)
        {
            throw new System.Exception("未找到蹲下模块");
        }
        
        actionModule.ExecuteModule(crouch);
    }
    
    /// <summary>
    /// 测试跳跃+冲刺组合
    /// </summary>
    void TestJumpDashCombination(ModuleTester tester)
    {
        var actions = new List<string> { "立定跳跃", "右冲刺" };
        actionModule.ExecuteActionSequence(actions, 0.3f);
    }
    
    /// <summary>
    /// 测试连续冲刺
    /// </summary>
    void TestContinuousDash(ModuleTester tester)
    {
        var actions = new List<string> { "右冲刺", "右冲刺", "右冲刺" };
        actionModule.ExecuteActionSequence(actions, 0.1f);
    }
    
    /// <summary>
    /// 测试复杂组合
    /// </summary>
    void TestComplexCombination(ModuleTester tester)
    {
        var actions = new List<string> { "右走", "右跳", "右冲刺", "蹲下" };
        actionModule.ExecuteActionSequence(actions, 0.2f);
    }
    
    /// <summary>
    /// 测试快速切换
    /// </summary>
    void TestRapidSwitching(ModuleTester tester)
    {
        StartCoroutine(RapidSwitchingCoroutine());
    }
    
    /// <summary>
    /// 测试长时间执行
    /// </summary>
    void TestLongExecution(ModuleTester tester)
    {
        var actions = new List<string> { "右走", "左走", "右走", "左走" };
        actionModule.ExecuteActionSequence(actions, 0.1f);
    }
    
    // ========== 辅助方法 ==========
    
    /// <summary>
    /// 延迟执行模块
    /// </summary>
    IEnumerator DelayedExecute(ActionModule.ActionModuleData module, float delay)
    {
        yield return new WaitForSeconds(delay);
        actionModule.ExecuteModule(module);
    }
    
    /// <summary>
    /// 顺序执行多个模块
    /// </summary>
    IEnumerator ExecuteSequentialModules(string[] moduleNames, float interval)
    {
        foreach (var moduleName in moduleNames)
        {
            var module = actionModule.FindModuleByName(moduleName);
            if (module != null)
            {
                actionModule.ExecuteModule(module);
                yield return new WaitForSeconds(interval);
            }
        }
    }
    
    /// <summary>
    /// 快速切换协程
    /// </summary>
    IEnumerator RapidSwitchingCoroutine()
    {
        var modules = new[] { "立定跳跃", "左冲刺", "右冲刺", "蹲下" };
        
        for (int i = 0; i < 5; i++) // 快速切换5次
        {
            foreach (var moduleName in modules)
            {
                var module = actionModule.FindModuleByName(moduleName);
                if (module != null)
                {
                    actionModule.ExecuteModule(module);
                    yield return new WaitForSeconds(0.1f); // 快速切换
                }
            }
        }
    }
    
    /// <summary>
    /// 在Scene视图中显示测试信息
    /// </summary>
    void OnGUI()
    {
        if (testCases.Count == 0) return;
        
        // 显示当前测试信息
        var currentTest = testCases[currentTestIndex];
        GUI.Label(new Rect(10, 500, 400, 20), $"当前测试: {currentTest.name}");
        GUI.Label(new Rect(10, 520, 400, 40), $"描述: {currentTest.description}");
        
        // 显示测试结果
        int yPos = 570;
        GUI.Label(new Rect(10, yPos, 200, 20), "=== 测试结果 ===");
        yPos += 25;
        
        foreach (var result in testResults)
        {
            string status = result.passed ? "✓" : "✗";
            string color = result.passed ? "green" : "red";
            GUI.color = result.passed ? Color.green : Color.red;
            
            GUI.Label(new Rect(10, yPos, 400, 20), 
                $"{status} {result.testName} - {result.message} ({result.executionTime:F3}s)");
            yPos += 20;
        }
        
        GUI.color = Color.white;
        
        // 显示控制提示
        yPos += 10;
        GUI.Label(new Rect(10, yPos, 400, 20), "控制: 1-9数字键快速测试, 空格键运行当前测试");
        yPos += 20;
        GUI.Label(new Rect(10, yPos, 400, 20), "左右箭头切换测试, R键重新运行, T键运行所有测试, C键清除结果");
    }
} 