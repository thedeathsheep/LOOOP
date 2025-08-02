using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动作模块系统 - 简化玩家的动作组合操作
/// </summary>
public class ActionModule : MonoBehaviour
{
    [System.Serializable]
    public class ActionModuleData
    {
        public string moduleName;           // 模块名称
        public string description;          // 模块描述
        public List<ActionStep> steps;      // 动作步骤列表
        public float totalDuration;         // 总持续时间
        public bool isLoopable;             // 是否可循环
        
        public ActionModuleData(string name, string desc, List<ActionStep> actionSteps, float duration = 0f, bool loopable = false)
        {
            moduleName = name;
            description = desc;
            steps = actionSteps;
            totalDuration = duration;
            isLoopable = loopable;
        }
    }
    
    [System.Serializable]
    public class ActionStep
    {
        public enum ActionType
        {
            Jump,       // 跳跃
            Dash,       // 冲刺
            Walk,       // 行走
            Crouch,     // 蹲下
            Idle,       // 静止
            Wait        // 等待
        }
        
        public ActionType type;
        public Vector2 direction;           // 动作方向
        public float duration;              // 持续时间
        public float delay;                 // 延迟时间（相对于前一个动作）
        
        public ActionStep(ActionType actionType, Vector2 dir, float dur = 0.1f, float del = 0f)
        {
            type = actionType;
            direction = dir;
            duration = dur;
            delay = del;
        }
    }
    
    [Header("基础动作模块")]
    public List<ActionModuleData> basicModules = new List<ActionModuleData>();
    
    [Header("当前状态")]
    public ActionModuleData currentModule;
    public bool isExecuting = false;
    public int currentStepIndex = 0;
    public float moduleStartTime;
    
    private MovementCommand movementCommand;
    
    void Start()
    {
        movementCommand = GetComponent<MovementCommand>();
        InitializeBasicModules();
    }
    
    /// <summary>
    /// 初始化基础动作模块
    /// </summary>
    void InitializeBasicModules()
    {
        basicModules.Add(CreateBasicJumpModule());
        basicModules.Add(CreateLeftJumpModule());
        basicModules.Add(CreateRightJumpModule());
        basicModules.Add(CreateLeftDashModule());
        basicModules.Add(CreateRightDashModule());
        basicModules.Add(CreateUpDashModule());
        basicModules.Add(CreateDownDashModule());
        basicModules.Add(CreateLeftWalkModule());
        basicModules.Add(CreateRightWalkModule());
        basicModules.Add(CreateCrouchModule());
        
        Debug.Log($"初始化了 {basicModules.Count} 个基础动作模块");
    }
    
    /// <summary>
    /// 执行指定的动作模块
    /// </summary>
    public void ExecuteModule(ActionModuleData module)
    {
        if (movementCommand == null)
        {
            Debug.LogError("未找到MovementCommand组件！");
            return;
        }
        
        currentModule = module;
        isExecuting = true;
        currentStepIndex = 0;
        moduleStartTime = Time.time;
        
        // 清空当前序列并添加新动作
        movementCommand.ClearSequence();
        
        foreach (var step in module.steps)
        {
            //把模组中的动作添加到指令序列中
            AddStepToSequence(step);
        }
        
        Debug.Log($"开始执行动作模块: {module.moduleName}");
        movementCommand.StartSequence();
    }
    
    /// <summary>
    /// 执行动作序列（动态组合）
    /// </summary>
    public void ExecuteActionSequence(List<string> actionNames)
    {
        ExecuteActionSequence(actionNames, 0.3f); // 默认等待0.3秒
    }
    
    /// <summary>
    /// 执行动作序列（动态组合）- 带自定义等待时间
    /// </summary>
    public void ExecuteActionSequence(List<string> actionNames, float jumpToDashDelay)
    {
        if (movementCommand == null)
        {
            Debug.LogError("未找到MovementCommand组件！");
            return;
        }
        
        // 清空当前序列
        movementCommand.ClearSequence();
        
        // 按顺序添加动作
        for (int i = 0; i < actionNames.Count; i++)
        {
            var actionName = actionNames[i];
            var module = FindModuleByName(actionName);
            if (module != null)
            {
                foreach (var step in module.steps)
                {
                    AddStepToSequence(step);
                }
                
                // 在跳跃后添加等待时间，让跳跃达到更高点
                if (IsJumpAction(actionName) && i < actionNames.Count - 1 && IsDashAction(actionNames[i + 1]))
                {
                    // 根据跳跃类型调整延迟时间
                    float adjustedDelay = GetAdjustedDelay(actionName, jumpToDashDelay);
                    AddWaitStep(adjustedDelay);
                }
            }
        }
        
        Debug.Log($"开始执行动作序列: {string.Join(" -> ", actionNames)} (跳跃到冲刺延迟: {jumpToDashDelay}s)");
        movementCommand.StartSequence();
    }
    
    /// <summary>
    /// 根据跳跃类型调整延迟时间
    /// </summary>
    private float GetAdjustedDelay(string jumpActionName, float baseDelay)
    {
        // 可以根据不同的跳跃类型调整延迟
        if (jumpActionName == "立定跳跃")
        {
            return baseDelay * 0.8f; // 立定跳跃稍微快一点
        }
        else if (jumpActionName.Contains("左跳") || jumpActionName.Contains("右跳"))
        {
            return baseDelay * 1.2f; // 方向跳跃需要更多时间达到最高点
        }
        
        return baseDelay; // 默认延迟
    }
    
    /// <summary>
    /// 判断是否为跳跃动作
    /// </summary>
    private bool IsJumpAction(string actionName)
    {
        return actionName.Contains("跳");
    }
    
    /// <summary>
    /// 判断是否为冲刺动作
    /// </summary>
    private bool IsDashAction(string actionName)
    {
        return actionName.Contains("冲刺");
    }
    
    /// <summary>
    /// 添加等待步骤
    /// </summary>
    private void AddWaitStep(float duration)
    {
        var waitStep = new ActionStep(ActionStep.ActionType.Wait, Vector2.zero, duration);
        AddStepToSequence(waitStep);
    }
    
    /// <summary>
    /// 将动作步骤添加到序列中
    /// </summary>
    void AddStepToSequence(ActionStep step)
    {
        switch (step.type)
        {
            case ActionStep.ActionType.Jump:
                movementCommand.AddJumpCommand(step.direction);
                break;
            case ActionStep.ActionType.Dash:
                movementCommand.AddDashCommand(step.direction);
                break;
            case ActionStep.ActionType.Walk:
                movementCommand.AddWalkCommand(step.direction, step.duration);
                break;
            case ActionStep.ActionType.Crouch:
                movementCommand.AddCrouchCommand(step.duration);
                break;
            case ActionStep.ActionType.Idle:
                movementCommand.AddIdleCommand(step.duration);
                break;
            case ActionStep.ActionType.Wait:
                movementCommand.AddIdleCommand(step.duration);
                break;
        }
    }
    
    /// <summary>
    /// 停止当前模块执行
    /// </summary>
    public void StopModule()
    {
        isExecuting = false;
        currentStepIndex = 0;
        if (movementCommand != null)
        {
            movementCommand.StopSequence();
        }
    }
    
    // ========== 基础动作模块创建方法 ==========
    
    ActionModuleData CreateBasicJumpModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Jump, Vector2.zero)
        };
        return new ActionModuleData("立定跳跃", "原地跳跃", steps, 0.1f);
    }
    
    ActionModuleData CreateLeftJumpModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Jump, Vector2.left)
        };
        return new ActionModuleData("左跳", "向左跳跃", steps, 0.1f);
    }
    
    ActionModuleData CreateRightJumpModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Jump, Vector2.right)
        };
        return new ActionModuleData("右跳", "向右跳跃", steps, 0.1f);
    }
    
    ActionModuleData CreateLeftDashModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Dash, Vector2.left)
        };
        return new ActionModuleData("左冲刺", "向左冲刺", steps, 0.1f);
    }
    
    ActionModuleData CreateRightDashModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Dash, Vector2.right)
        };
        return new ActionModuleData("右冲刺", "向右冲刺", steps, 0.1f);
    }
    
    ActionModuleData CreateUpDashModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Dash, Vector2.up)
        };
        return new ActionModuleData("上冲刺", "向上冲刺", steps, 0.1f);
    }
    
    ActionModuleData CreateDownDashModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Dash, Vector2.down)
        };
        return new ActionModuleData("下冲刺", "向下冲刺", steps, 0.1f);
    }
    
    ActionModuleData CreateLeftWalkModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Walk, Vector2.left, 1f)
        };
        return new ActionModuleData("左走", "向左行走99秒", steps, 99f);
    }
    
    ActionModuleData CreateRightWalkModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Walk, Vector2.right, 1f)
        };
        return new ActionModuleData("右走", "向右行走99秒", steps, 99f);
    }
    
    ActionModuleData CreateCrouchModule()
    {
        var steps = new List<ActionStep>
        {
            new ActionStep(ActionStep.ActionType.Crouch, Vector2.zero, 1f)
        };
        return new ActionModuleData("蹲下", "蹲下1秒", steps, 1f);
    }
    
    /// <summary>
    /// 获取所有可用模块的列表
    /// </summary>
    public List<ActionModuleData> GetAvailableModules()
    {
        return basicModules;
    }
    
    /// <summary>
    /// 根据名称查找模块
    /// </summary>
    public ActionModuleData FindModuleByName(string moduleName)
    {
        return basicModules.Find(m => m.moduleName == moduleName);
    }
    
    /// <summary>
    /// 创建动态组合模块
    /// </summary>
    public ActionModuleData CreateDynamicModule(string name, List<string> actionSequence)
    {
        var steps = new List<ActionStep>();
        float totalDuration = 0f;
        
        foreach (var actionName in actionSequence)
        {
            var module = FindModuleByName(actionName);
            if (module != null)
            {
                steps.AddRange(module.steps);
                totalDuration += module.totalDuration;
            }
        }
        
        return new ActionModuleData(name, $"动态组合: {string.Join(" -> ", actionSequence)}", steps, totalDuration);
    }
    
    void OnGUI()
    {
        if (isExecuting && currentModule != null)
        {
            GUI.Label(new Rect(10, 400, 300, 20), $"执行模块: {currentModule.moduleName}");
            GUI.Label(new Rect(10, 420, 300, 20), $"描述: {currentModule.description}");
            GUI.Label(new Rect(10, 440, 300, 20), $"步骤: {currentStepIndex + 1}/{currentModule.steps.Count}");
        }
    }
} 