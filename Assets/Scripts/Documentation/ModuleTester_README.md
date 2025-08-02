# ActionModule 测试器使用说明

## 概述

这个测试系统包含两个测试器脚本，用于测试ActionModule的各种功能：

1. **ModuleTester.cs** - 完整的测试框架，包含自动化测试和详细的结果记录
2. **SimpleModuleTester.cs** - 简单直观的测试器，适合快速测试和调试

## 安装和使用

### 1. 添加测试器组件

1. 在玩家GameObject上添加 `SimpleModuleTester` 或 `ModuleTester` 组件
2. 确保该GameObject已经有 `ActionModule` 组件
3. 在Inspector中将ActionModule组件拖拽到测试器的 `actionModule` 字段

### 2. SimpleModuleTester 使用说明

#### 快捷键控制

| 按键 | 功能 |
|------|------|
| **数字键1-0** | 快速测试常用模块（1=立定跳跃, 2=左跳, 3=右跳, 4=左冲刺, 5=右冲刺, 6=上冲刺, 7=下冲刺, 8=左走, 9=右走, 0=蹲下） |
| **J** | 立定跳跃 |
| **L** | 左跳 |
| **R** | 右跳 |
| **A** | 左冲刺 |
| **D** | 右冲刺 |
| **W** | 上冲刺 |
| **S** | 下冲刺 |
| **C** | 蹲下 |
| **空格** | 跳跃+冲刺组合 |
| **X** | 停止当前测试 |
| **ESC** | 清除所有测试 |

#### 右键菜单

在Scene视图中选中测试器GameObject，右键可以看到以下测试选项：
- 测试立定跳跃
- 测试左跳
- 测试右跳
- 测试右冲刺
- 测试左冲刺
- 测试蹲下
- 测试跳跃+冲刺组合
- 停止当前测试

### 3. ModuleTester 使用说明

#### 测试用例

ModuleTester包含以下测试用例：

1. **基础动作测试**
   - 基础跳跃测试
   - 方向跳跃测试
   - 冲刺测试
   - 行走测试
   - 蹲下测试

2. **组合动作测试**
   - 跳跃+冲刺组合
   - 连续冲刺
   - 复杂组合

3. **性能测试**
   - 快速切换测试
   - 长时间执行

#### 控制方式

| 按键 | 功能 |
|------|------|
| **数字键1-9** | 运行对应索引的测试 |
| **空格** | 运行当前选中的测试 |
| **左右箭头** | 切换测试用例 |
| **R** | 重新运行当前测试 |
| **T** | 运行所有测试 |
| **C** | 清除测试结果 |

## 测试功能详解

### 1. 单个模块测试

测试ActionModule中定义的基础动作模块：

```csharp
// 测试立定跳跃
tester.TestModule("立定跳跃");

// 测试方向跳跃
tester.TestModule("左跳");
tester.TestModule("右跳");

// 测试冲刺
tester.TestModule("左冲刺");
tester.TestModule("右冲刺");
tester.TestModule("上冲刺");
tester.TestModule("下冲刺");
```

### 2. 组合动作测试

测试动作序列的组合：

```csharp
// 跳跃+冲刺组合
var actions = new List<string> { "立定跳跃", "右冲刺" };
actionModule.ExecuteActionSequence(actions, 0.3f);

// 连续冲刺
var actions = new List<string> { "右冲刺", "右冲刺", "右冲刺" };
actionModule.ExecuteActionSequence(actions, 0.1f);

// 复杂组合
var actions = new List<string> { "右走", "右跳", "右冲刺", "蹲下" };
actionModule.ExecuteActionSequence(actions, 0.2f);
```

### 3. 性能测试

测试系统的稳定性和性能：

- **快速切换测试**: 快速切换不同动作，测试系统响应性
- **长时间执行**: 长时间执行动作序列，测试系统稳定性

## 测试结果

### SimpleModuleTester

- 实时显示当前测试状态
- 显示测试运行时间
- 提供直观的UI界面

### ModuleTester

- 详细的测试结果记录
- 测试通过/失败状态
- 执行时间统计
- 测试总结报告

## 调试技巧

### 1. 观察测试效果

1. 在Scene视图中运行游戏
2. 使用快捷键触发测试
3. 观察角色的动作表现
4. 查看Console中的调试信息

### 2. 调整测试参数

在Inspector中可以调整以下参数：

- `autoTest`: 是否自动运行测试
- `testInterval`: 测试间隔时间
- `showTestUI`: 是否显示测试UI
- `autoStopAfterTest`: 测试后是否自动停止

### 3. 自定义测试

可以修改测试脚本添加自定义测试：

```csharp
// 在SimpleModuleTester中添加自定义测试方法
public void TestCustomAction()
{
    var actions = new List<string> { "自定义动作1", "自定义动作2" };
    actionModule.ExecuteActionSequence(actions, 0.5f);
}
```

## 常见问题

### 1. 测试器不工作

- 检查ActionModule组件是否正确引用
- 确认ActionModule中有对应的模块定义
- 查看Console中的错误信息

### 2. 动作执行异常

- 检查PlayerMovement组件是否正常工作
- 确认MovementCommand系统是否正常
- 验证输入配置是否正确

### 3. 测试结果不准确

- 检查测试时间设置是否合理
- 确认自动停止功能是否正常工作
- 验证测试逻辑是否正确

## 扩展功能

### 1. 添加新的测试用例

在ModuleTester的`InitializeTestCases()`方法中添加新的测试用例：

```csharp
testCases.Add(new TestCase("新测试", "测试描述", TestNewFunction));
```

### 2. 自定义测试逻辑

创建新的测试方法：

```csharp
void TestNewFunction(ModuleTester tester)
{
    // 自定义测试逻辑
    var module = actionModule.FindModuleByName("模块名称");
    actionModule.ExecuteModule(module);
}
```

### 3. 集成到其他系统

可以将测试器集成到其他系统中：

```csharp
// 在其他脚本中调用测试
public SimpleModuleTester tester;

void Start()
{
    tester = GetComponent<SimpleModuleTester>();
}

void TestSomething()
{
    tester.TestModule("模块名称");
}
```

## 总结

这个测试系统提供了完整的ActionModule功能测试方案，包括：

- 快速测试工具
- 自动化测试框架
- 详细的测试报告
- 直观的用户界面
- 灵活的扩展性

通过使用这些测试工具，可以有效地验证ActionModule的各种功能，确保系统的稳定性和可靠性。 