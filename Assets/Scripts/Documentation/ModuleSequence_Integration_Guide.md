# 模块序列功能对接说明文档

## 概述

本文档描述了如何开发一个能够创造模块序列的功能，该功能将允许用户创建自定义的动作序列，并在游戏中循环执行这些序列。

## 系统架构

### 核心组件

1. **ActionModule** (`Assets/Scripts/Player/ActionModule.cs`)
   - 负责管理动作模块的定义和执行
   - 提供基础动作模块库
   - 支持动态组合模块

2. **MovementCommand** (`Assets/Scripts/Player/MovementCommand.cs`)
   - 负责执行具体的运动指令序列
   - 模拟玩家输入
   - 管理指令的执行时序

3. **PlayerMovement** (`Assets/Scripts/Player/PlayerMovement.cs`)
   - 处理实际的玩家移动逻辑
   - 响应模拟输入

## 数据结构

### ActionModuleData
```csharp
[System.Serializable]
public class ActionModuleData
{
    public string moduleName;           // 模块名称
    public string description;          // 模块描述
    public List<ActionStep> steps;      // 动作步骤列表
    public float totalDuration;         // 总持续时间
    public bool isLoopable;             // 是否可循环
}
```

### ActionStep
```csharp
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
    public float delay;                 // 延迟时间
}
```

### Command
```csharp
[System.Serializable]
public class Command
{
    public enum CommandType
    {
        Jump,       // 跳跃动作
        Dash,       // 冲刺动作
        Crouch,     // 蹲下动作
        Walk,       // 行走动作
        Idle        // 静止动作
    }

    public CommandType type;
    public float duration;
    public Vector2 direction;
    public bool isExecuting;
    public float startTime;
    public bool hasTriggeredAction;
}
```

## 基础动作模块

系统已预定义了以下基础动作模块：

| 模块名称 | 描述 | 动作类型 | 方向 |
|---------|------|----------|------|
| 立定跳跃 | 原地跳跃 | Jump | Vector2.zero |
| 左跳 | 向左跳跃 | Jump | Vector2.left |
| 右跳 | 向右跳跃 | Jump | Vector2.right |
| 左冲刺 | 向左冲刺 | Dash | Vector2.left |
| 右冲刺 | 向右冲刺 | Dash | Vector2.right |
| 上冲刺 | 向上冲刺 | Dash | Vector2.up |
| 下冲刺 | 向下冲刺 | Dash | Vector2.down |
| 左走 | 向左行走 | Walk | Vector2.left |
| 右走 | 向右行走 | Walk | Vector2.right |
| 蹲下 | 蹲下动作 | Crouch | Vector2.zero |

## 开发指南

### 1. 创建模块序列管理器

建议创建一个新的脚本 `ModuleSequenceManager.cs`：

```csharp
using System.Collections.Generic;
using UnityEngine;

public class ModuleSequenceManager : MonoBehaviour
{
    [Header("序列管理")]
    public List<ActionModule.ActionModuleData> customSequences = new List<ActionModule.ActionModuleData>();
    public ActionModule.ActionModuleData currentSequence;
    public bool isLooping = false;
    
    private ActionModule actionModule;
    
    void Start()
    {
        actionModule = GetComponent<ActionModule>();
    }
    
    // 创建新的模块序列
    public ActionModule.ActionModuleData CreateSequence(string name, List<string> actionNames)
    {
        var sequence = actionModule.CreateDynamicModule(name, actionNames);
        customSequences.Add(sequence);
        return sequence;
    }
    
    // 执行序列
    public void ExecuteSequence(ActionModule.ActionModuleData sequence)
    {
        currentSequence = sequence;
        actionModule.ExecuteModule(sequence);
    }
    
    // 循环执行序列
    public void LoopSequence(ActionModule.ActionModuleData sequence)
    {
        isLooping = true;
        // 实现循环逻辑
    }
}
```

### 2. 序列创建接口

提供以下方法来创建和管理序列：

#### 2.1 创建简单序列
```csharp
public ActionModule.ActionModuleData CreateSimpleSequence(string name, params string[] actionNames)
{
    var actionList = new List<string>(actionNames);
    return CreateSequence(name, actionList);
}
```

#### 2.2 创建带延迟的序列
```csharp
public ActionModule.ActionModuleData CreateSequenceWithDelays(string name, 
    List<(string action, float delay)> actionsWithDelays)
{
    var steps = new List<ActionModule.ActionStep>();
    
    foreach (var (actionName, delay) in actionsWithDelays)
    {
        var module = actionModule.FindModuleByName(actionName);
        if (module != null)
        {
            // 添加延迟
            if (delay > 0)
            {
                steps.Add(new ActionModule.ActionStep(ActionModule.ActionStep.ActionType.Wait, Vector2.zero, delay));
            }
            // 添加动作
            steps.AddRange(module.steps);
        }
    }
    
    return new ActionModule.ActionModuleData(name, $"带延迟序列: {name}", steps);
}
```

#### 2.3 创建循环序列
```csharp
public ActionModule.ActionModuleData CreateLoopingSequence(string name, List<string> actionNames, int loopCount)
{
    var steps = new List<ActionModule.ActionStep>();
    
    for (int i = 0; i < loopCount; i++)
    {
        foreach (var actionName in actionNames)
        {
            var module = actionModule.FindModuleByName(actionName);
            if (module != null)
            {
                steps.AddRange(module.steps);
            }
        }
    }
    
    return new ActionModule.ActionModuleData(name, $"循环序列: {name} x{loopCount}", steps, 0f, true);
}
```

### 3. 序列执行控制

#### 3.1 基本执行控制
```csharp
// 开始执行序列
public void StartSequence(ActionModule.ActionModuleData sequence)
{
    actionModule.ExecuteModule(sequence);
}

// 停止当前序列
public void StopSequence()
{
    actionModule.StopModule();
}

// 暂停/恢复序列
public void PauseSequence()
{
    // 实现暂停逻辑
}

public void ResumeSequence()
{
    // 实现恢复逻辑
}
```

#### 3.2 循环执行
```csharp
public void StartLoopingSequence(ActionModule.ActionModuleData sequence)
{
    // 设置循环标志
    sequence.isLoopable = true;
    
    // 开始执行
    StartSequence(sequence);
    
    // 监听序列完成事件，自动重新开始
    StartCoroutine(LoopSequenceCoroutine(sequence));
}

private IEnumerator LoopSequenceCoroutine(ActionModule.ActionModuleData sequence)
{
    while (sequence.isLoopable)
    {
        // 等待序列完成
        yield return new WaitUntil(() => !actionModule.isExecuting);
        
        // 重新开始序列
        if (sequence.isLoopable)
        {
            StartSequence(sequence);
        }
    }
}
```

### 4. 序列编辑功能

#### 4.1 序列编辑器
```csharp
public class SequenceEditor : MonoBehaviour
{
    [Header("编辑界面")]
    public List<string> availableActions = new List<string>();
    public List<string> currentSequence = new List<string>();
    
    // 添加动作到序列
    public void AddActionToSequence(string actionName)
    {
        if (availableActions.Contains(actionName))
        {
            currentSequence.Add(actionName);
        }
    }
    
    // 从序列中移除动作
    public void RemoveActionFromSequence(int index)
    {
        if (index >= 0 && index < currentSequence.Count)
        {
            currentSequence.RemoveAt(index);
        }
    }
    
    // 移动动作位置
    public void MoveActionInSequence(int fromIndex, int toIndex)
    {
        if (fromIndex >= 0 && fromIndex < currentSequence.Count &&
            toIndex >= 0 && toIndex < currentSequence.Count)
        {
            var action = currentSequence[fromIndex];
            currentSequence.RemoveAt(fromIndex);
            currentSequence.Insert(toIndex, action);
        }
    }
    
    // 保存序列
    public void SaveSequence(string name)
    {
        var sequenceManager = FindObjectOfType<ModuleSequenceManager>();
        if (sequenceManager != null)
        {
            sequenceManager.CreateSequence(name, new List<string>(currentSequence));
        }
    }
}
```

### 5. 序列验证

#### 5.1 验证序列有效性
```csharp
public bool ValidateSequence(List<string> actionNames)
{
    foreach (var actionName in actionNames)
    {
        if (actionModule.FindModuleByName(actionName) == null)
        {
            Debug.LogError($"无效的动作名称: {actionName}");
            return false;
        }
    }
    return true;
}
```

#### 5.2 检查序列兼容性
```csharp
public bool CheckSequenceCompatibility(List<string> actionNames)
{
    for (int i = 0; i < actionNames.Count - 1; i++)
    {
        var currentAction = actionNames[i];
        var nextAction = actionNames[i + 1];
        
        // 检查动作组合是否合理
        if (!IsActionCompatible(currentAction, nextAction))
        {
            Debug.LogWarning($"动作组合可能不合理: {currentAction} -> {nextAction}");
            return false;
        }
    }
    return true;
}

private bool IsActionCompatible(string action1, string action2)
{
    // 实现动作兼容性检查逻辑
    // 例如：连续跳跃可能不合理
    if (action1.Contains("跳") && action2.Contains("跳"))
    {
        return false;
    }
    return true;
}
```

## 使用示例

### 示例1：创建简单序列
```csharp
// 创建"跳跃冲刺"序列
var sequenceManager = GetComponent<ModuleSequenceManager>();
var jumpDashSequence = sequenceManager.CreateSimpleSequence("跳跃冲刺", "立定跳跃", "右冲刺");

// 执行序列
sequenceManager.ExecuteSequence(jumpDashSequence);
```

### 示例2：创建循环序列
```csharp
// 创建循环移动序列
var moveSequence = sequenceManager.CreateLoopingSequence("循环移动", 
    new List<string> { "右走", "左走" }, 5);

// 开始循环执行
sequenceManager.StartLoopingSequence(moveSequence);
```

### 示例3：使用序列编辑器
```csharp
var editor = GetComponent<SequenceEditor>();

// 添加动作到序列
editor.AddActionToSequence("立定跳跃");
editor.AddActionToSequence("右冲刺");
editor.AddActionToSequence("右走");

// 保存序列
editor.SaveSequence("我的自定义序列");
```

## 注意事项

1. **性能考虑**
   - 避免创建过长的序列，可能导致性能问题
   - 考虑序列执行的内存占用

2. **错误处理**
   - 始终验证序列的有效性
   - 处理无效动作名称的情况
   - 提供序列执行失败的回调

3. **用户体验**
   - 提供序列预览功能
   - 允许用户随时停止/暂停序列
   - 提供序列执行进度显示

4. **扩展性**
   - 设计时考虑未来可能添加的新动作类型
   - 保持接口的向后兼容性

## 测试建议

1. **单元测试**
   - 测试序列创建功能
   - 测试序列验证逻辑
   - 测试动作兼容性检查

2. **集成测试**
   - 测试序列执行流程
   - 测试循环执行功能
   - 测试序列编辑器功能

3. **性能测试**
   - 测试长序列的执行性能
   - 测试多个序列同时执行的情况

## 联系方式

如有问题，请联系：
- 项目负责人：[姓名]
- 技术支持：[邮箱]
- 文档版本：v1.0
- 最后更新：[日期]

---

## 模块功能对接说明

### 核心开发任务

你的同事需要开发一个**模块序列创建器**，该系统将与现有的 `ActionModule` 和 `MovementCommand` 系统集成。

### 关键接口

**ActionModule 提供的方法：**
- `ExecuteModule(ActionModuleData module)` - 执行模块
- `CreateDynamicModule(string name, List<string> actionSequence)` - 创建动态模块
- `FindModuleByName(string moduleName)` - 查找模块
- `GetAvailableModules()` - 获取所有可用模块

**MovementCommand 提供的方法：**
- `StartSequence()` - 开始执行序列
- `StopSequence()` - 停止执行序列
- `ClearSequence()` - 清空序列

### 开发建议

1. **创建UI界面** - 让用户能够拖拽组合动作
2. **序列验证** - 检查动作组合的合理性
3. **保存/加载** - 支持序列的持久化存储
4. **实时预览** - 显示序列执行效果
5. **错误处理** - 处理无效序列和异常情况

### 测试要点

- 测试序列创建功能
- 测试循环执行逻辑
- 测试序列中断和恢复
- 测试多个序列的切换
- 测试性能（长序列、多循环）

这个系统将为游戏提供强大的动作序列创建和执行能力，让玩家能够设计复杂的动作组合并在游戏中循环执行。

---

## 模块功能对接说明

### 核心开发任务

你的同事需要开发一个**模块序列创建器**，该系统将与现有的 `ActionModule` 和 `MovementCommand` 系统集成。

### 关键接口

**ActionModule 提供的方法：**
- `ExecuteModule(ActionModuleData module)` - 执行模块
- `CreateDynamicModule(string name, List<string> actionSequence)` - 创建动态模块
- `FindModuleByName(string moduleName)` - 查找模块
- `GetAvailableModules()` - 获取所有可用模块

**MovementCommand 提供的方法：**
- `StartSequence()` - 开始执行序列
- `StopSequence()` - 停止执行序列
- `ClearSequence()` - 清空序列

### 开发建议

1. **创建UI界面** - 让用户能够拖拽组合动作
2. **序列验证** - 检查动作组合的合理性
3. **保存/加载** - 支持序列的持久化存储
4. **实时预览** - 显示序列执行效果
5. **错误处理** - 处理无效序列和异常情况

### 测试要点

- 测试序列创建功能
- 测试循环执行逻辑
- 测试序列中断和恢复
- 测试多个序列的切换
- 测试性能（长序列、多循环）

这个系统将为游戏提供强大的动作序列创建和执行能力，让玩家能够设计复杂的动作组合并在游戏中循环执行。 