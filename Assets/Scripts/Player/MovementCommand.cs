using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 运动指令系统 - 用于模拟玩家输入和执行预定义的运动序列
/// </summary>
public class MovementCommand : MonoBehaviour
{
    [System.Serializable]
    public class Command
    {
        public enum CommandType
        {
            Jump,       // 跳跃动作（按下跳跃键）
            Dash,       // 冲刺动作（按下冲刺键）
            Crouch,     // 蹲下动作（按住蹲下键）
            Walk,       // 行走动作（按住方向键）
            Idle        // 静止动作（不按任何键）
        }

        public CommandType type;           // 指令类型
        public float duration;             // 指令持续时间（秒）
        public Vector2 direction;          // 指令方向
        public bool isExecuting = false;   // 是否正在执行
        public float startTime;            // 开始执行时间
        public bool hasTriggeredAction = false; // 是否已经触发了动作（用于一次性动作如跳跃）

        public Command(CommandType cmdType, float cmdDuration, Vector2 cmdDirection)
        {
            type = cmdType;
            duration = cmdDuration;
            direction = cmdDirection;
        }
    }

    [Header("指令序列设置")]
    public List<Command> commandSequence = new List<Command>(); // 指令序列，这里添加指令时，需要添加对应的CommandType
    public bool autoExecute = false;       // 是否自动执行
    public bool loopSequence = false;      // 是否循环执行序列
    public float sequenceDelay = 0f;       // 序列开始前的延迟

    [Header("当前状态")]
    public int currentCommandIndex = 0;    // 当前执行的指令索引
    public bool isExecutingSequence = false; // 是否正在执行序列
    public float sequenceStartTime;        // 序列开始时间

    // 模拟输入状态
    private bool simulatedJump = false;
    private bool simulatedDash = false;
    private bool simulatedCrouch = false;
    private Vector2 simulatedDirection = Vector2.zero;

    // 组件引用
    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (autoExecute)
        {
            StartCoroutine(ExecuteSequenceWithDelay());
        }
    }

    void Update()
    {
        if (isExecutingSequence)
        {
            UpdateCurrentCommand();
        }
    }

    /// <summary>
    /// 添加跳跃指令（一次性执行）
    /// </summary>
    /// <param name="direction">跳跃方向 (Vector2.zero=立定跳, Vector2.left=左跳, Vector2.right=右跳)</param>
    public void AddJumpCommand(Vector2 direction = default)
    {
        commandSequence.Add(new Command(Command.CommandType.Jump, 0.1f, direction));
    }

    /// <summary>
    /// 添加冲刺指令（一次性执行）
    /// </summary>
    /// <param name="direction">冲刺方向</param>
    public void AddDashCommand(Vector2 direction)
    {
        commandSequence.Add(new Command(Command.CommandType.Dash, 0.1f, direction));
    }

    /// <summary>
    /// 添加蹲下指令
    /// </summary>
    /// <param name="duration">持续时间</param>
    public void AddCrouchCommand(float duration = 1f)
    {
        commandSequence.Add(new Command(Command.CommandType.Crouch, duration, Vector2.zero));
    }

    /// <summary>
    /// 添加行走指令
    /// </summary>
    /// <param name="direction">行走方向</param>
    /// <param name="duration">持续时间</param>
    public void AddWalkCommand(Vector2 direction, float duration = 1f)
    {
        commandSequence.Add(new Command(Command.CommandType.Walk, duration, direction));
    }

    /// <summary>
    /// 添加静止指令
    /// </summary>
    /// <param name="duration">持续时间</param>
    public void AddIdleCommand(float duration = 1f)
    {
        commandSequence.Add(new Command(Command.CommandType.Idle, duration, Vector2.zero));
    }



    /// <summary>
    /// 清空指令序列
    /// </summary>
    public void ClearSequence()
    {
        commandSequence.Clear();
        currentCommandIndex = 0;
        isExecutingSequence = false;
        ResetSimulatedInputs();
    }

    /// <summary>
    /// 开始执行指令序列
    /// </summary>
    public void StartSequence()
    {
        if (commandSequence.Count > 0)
        {
            isExecutingSequence = true;
            currentCommandIndex = 0;
            sequenceStartTime = Time.time;
            
            // 开始执行第一个指令
            if (currentCommandIndex < commandSequence.Count)
            {
                ExecuteCommand(commandSequence[currentCommandIndex]);
            }
        }
    }

    /// <summary>
    /// 停止执行指令序列
    /// </summary>
    public void StopSequence()
    {
        isExecutingSequence = false;
        currentCommandIndex = 0;
        ResetSimulatedInputs();
    }

    /// <summary>
    /// 带延迟执行序列的协程
    /// </summary>
    public IEnumerator ExecuteSequenceWithDelay()
    {
        if (sequenceDelay > 0)
        {
            yield return new WaitForSeconds(sequenceDelay);
        }
        StartSequence();
    }

    /// <summary>
    /// 更新当前指令
    /// </summary>
    private void UpdateCurrentCommand()
    {
        if (currentCommandIndex >= commandSequence.Count)
        {
            // 序列执行完毕
            if (loopSequence)
            {
                // 循环执行
                currentCommandIndex = 0;
                sequenceStartTime = Time.time;
                if (currentCommandIndex < commandSequence.Count)
                {
                    ExecuteCommand(commandSequence[currentCommandIndex]);
                }
            }
            else
            {
                // 停止执行
                StopSequence();
            }
            return;
        }

        Command currentCommand = commandSequence[currentCommandIndex];
        
        // 检查当前指令是否执行完毕
        if (Time.time - currentCommand.startTime >= currentCommand.duration)
        {
            // 当前指令执行完毕，移动到下一个指令
            currentCommand.isExecuting = false;
            currentCommand.hasTriggeredAction = false;
            
            // 如果当前指令是蹲下，在结束时自动重置蹲下状态
            if (currentCommand.type == Command.CommandType.Crouch)
            {
                simulatedCrouch = false;
                Debug.Log("蹲下指令结束，重置蹲下状态");
            }
            
            currentCommandIndex++;
            
            // 注意：Jump指令结束时不再重置方向，而是由PlayerMovement的落地检测来处理
            // 这样可以避免指令系统对移动的即时干扰
            
            if (currentCommandIndex < commandSequence.Count)
            {
                ExecuteCommand(commandSequence[currentCommandIndex]);
            }
            else
            {
                // 序列结束，重置所有模拟输入
                ResetSimulatedInputs();
            }
        }
        else
        {
            // 指令仍在执行中，确保模拟输入状态正确
            UpdateSimulatedInputs(currentCommand);
        }
    }

    /// <summary>
    /// 执行单个指令
    /// </summary>
    /// <param name="command">要执行的指令</param>
    private void ExecuteCommand(Command command)
    {
        command.isExecuting = true;
        command.startTime = Time.time;
        command.hasTriggeredAction = false; // 重置动作触发标志
        
        Debug.Log($"开始执行指令: {command.type}, 方向: {command.direction}, 持续时间: {command.duration}s");
        
        // 根据指令类型设置模拟输入状态
        UpdateSimulatedInputs(command);
    }

    /// <summary>
    /// 更新模拟输入状态
    /// </summary>
    /// <param name="command">当前执行的指令</param>
    private void UpdateSimulatedInputs(Command command)
    {
        switch (command.type)
        {
            case Command.CommandType.Jump:
                // 跳跃指令：一次性执行，设置跳跃状态和方向
                simulatedJump = true;
                simulatedDirection = command.direction; // 跳跃时使用方向
                Debug.Log($"Jump指令 - 一次性执行，方向: {simulatedDirection}, dirX: {simulatedDirection.x}, 跳跃状态: {simulatedJump}");
                break;
                
            case Command.CommandType.Dash:
                // 冲刺指令：一次性执行
                simulatedDash = true;
                simulatedDirection = command.direction;
                Debug.Log($"Dash指令 - 一次性执行，方向: {simulatedDirection}");
                break;
                
            case Command.CommandType.Crouch:
                // 蹲下指令：模拟按住蹲下键
                simulatedCrouch = true;
                simulatedDirection = Vector2.zero; // 蹲下时不需要方向
                break;
                
            case Command.CommandType.Walk:
                // 行走指令：模拟按住方向键
                simulatedJump = false;
                simulatedDash = false;
                simulatedCrouch = false;
                simulatedDirection = command.direction;
                Debug.Log($"Walk指令 - 设置方向: {simulatedDirection}");
                break;
                
            case Command.CommandType.Idle:
                // 静止指令：不按任何键
                simulatedJump = false;
                simulatedDash = false;
                simulatedCrouch = false;
                simulatedDirection = Vector2.zero;
                Debug.Log($"Idle指令 - 重置所有输入，方向: {simulatedDirection}");
                break;
                

        }
    }

    /// <summary>
    /// 重置模拟输入状态
    /// </summary>
    private void ResetSimulatedInputs()
    {
        simulatedJump = false;
        simulatedDash = false;
        simulatedCrouch = false;
        simulatedDirection = Vector2.zero;
    }

    // 公共访问方法
    public bool IsExecutingCommand()
    {
        return isExecutingSequence;
    }

    public bool GetSimulatedJump()
    {
        return simulatedJump;
    }

    public bool GetSimulatedDash()
    {
        return simulatedDash;
    }

    public bool GetSimulatedCrouch()
    {
        return simulatedCrouch;
    }

    public float GetSimulatedDirX()
    {
        return simulatedDirection.x;
    }

    public float GetSimulatedDirY()
    {
        return simulatedDirection.y;
    }

    /// <summary>
    /// 玩家落地时的回调方法
    /// 由PlayerMovement调用，用于在落地时重置方向输入并执行下一个指令
    /// </summary>
    public void OnPlayerLanded()
    {
        // 落地时重置方向输入
        simulatedDirection = Vector2.zero;
        Debug.Log($"玩家落地 - 重置方向输入: {simulatedDirection}");
        
        // 如果当前正在执行跳跃指令，立即结束当前指令并执行下一个
        if (isExecutingSequence && currentCommandIndex < commandSequence.Count)
        {
            Command currentCommand = commandSequence[currentCommandIndex];
            if (currentCommand.type == Command.CommandType.Jump)
            {
                Debug.Log("检测到跳跃指令执行中，落地后立即执行下一个指令");
                // 强制结束当前跳跃指令
                currentCommand.isExecuting = false;
                currentCommand.hasTriggeredAction = false;
                currentCommandIndex++;
                
                // 执行下一个指令
                if (currentCommandIndex < commandSequence.Count)
                {
                    ExecuteCommand(commandSequence[currentCommandIndex]);
                }
                else
                {
                    // 序列结束，重置所有模拟输入
                    ResetSimulatedInputs();
                }
            }
        }
    }

    /// <summary>
    /// 创建示例指令序列（右键菜单）
    /// </summary>
    [ContextMenu("创建示例序列")]
    public void CreateExampleSequence()
    {
        ClearSequence();
        AddWalkCommand(new Vector2(1, 0), 2f);    // 向右行走2秒
        AddJumpCommand(Vector2.right);             // 向右跳跃（一次性执行）
        AddIdleCommand(0.5f);                     // 静止0.5秒
        AddDashCommand(new Vector2(1, 0));        // 向右冲刺（一次性执行）
        AddIdleCommand(1f);                       // 静止1秒
        AddCrouchCommand(1f);                     // 蹲下1秒
    }

    /// <summary>
    /// 在Scene视图中显示调试信息
    /// </summary>
    void OnGUI()
    {
        if (isExecutingSequence)
        {
            GUI.Label(new Rect(10, 10, 300, 20), $"执行指令序列: {currentCommandIndex + 1}/{commandSequence.Count}");
            if (currentCommandIndex < commandSequence.Count)
            {
                Command current = commandSequence[currentCommandIndex];
                float remainingTime = current.duration - (Time.time - current.startTime);
                GUI.Label(new Rect(10, 30, 300, 20), $"当前指令: {current.type}, 剩余时间: {remainingTime:F2}s");
                GUI.Label(new Rect(10, 50, 300, 20), $"模拟输入 - 跳跃: {simulatedJump}, 冲刺: {simulatedDash}, 蹲下: {simulatedCrouch}");
                GUI.Label(new Rect(10, 70, 300, 20), $"方向: ({simulatedDirection.x:F1}, {simulatedDirection.y:F1})");
            }
        }
    }
} 