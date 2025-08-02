using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家移动控制器 - 控制玩家的五个基础动作：冲刺、跳跃、行走、静止、蹲下
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    // 输入相关变量
    private enum KeyState { Off, Held, Up, Down } // 按键状态枚举：关闭、按住、抬起、按下
    private KeyState tempKeyJump; // 跳跃键的临时状态
    private KeyState keyJump; // 跳跃键的当前状态
    private KeyState tempKeyDash; // 冲刺键的临时状态
    private KeyState keyDash; // 冲刺键的当前状态
    private KeyState tempKeyCrouch; // 蹲下键的临时状态
    private KeyState keyCrouch; // 蹲下键的当前状态
    public float dirX; // 水平输入方向
    public float dirY; // 垂直输入方向
    
    // 运动指令系统相关
    private MovementCommand movementCommand; // 运动指令组件
    private bool useSimulatedInput = false; // 是否使用模拟输入
    

    
    // 物理组件
    private Rigidbody2D rb; // 刚体组件
    [HideInInspector] public BoxCollider2D coll; // 碰撞体组件（隐藏于Inspector）
    private StopObject stop; // 停止对象组件
    public LayerMask wall; // 墙壁层级掩码

    // 状态变量
    public bool facingLeft = false; // 是否面向左侧
    public bool isAirborne; // 是否在空中
    private DeathAndRespawn deathResp; // 死亡重生组件

    // 速度相关变量
    [SerializeField] private float moveSpeed = 7f; // 移动速度（可在Inspector中调整）
    [SerializeField] private float jumpForce = 8f; // 跳跃力度（可在Inspector中调整）
    [SerializeField] private float maxVerticalSpeed = 15f; // 最大垂直速度（可在Inspector中调整）
    public float gravityScale; // 重力缩放

    // 冲刺相关变量
    [SerializeField] private int dashDuration = 8; // 冲刺持续时间（可在Inspector中调整）
    [SerializeField] private float dashSpeed = 12f; // 冲刺速度（可在Inspector中调整）
    [HideInInspector] public Vector2 dashDirection = Vector2.zero; // 冲刺方向（隐藏于Inspector）
    public int dashState = 0; // 冲刺状态
    [HideInInspector] public bool canDash = true; // 是否可以冲刺（隐藏于Inspector）
    [HideInInspector] public bool isDashing = false; // 是否正在冲刺（隐藏于Inspector）

    // 蹲下相关变量
    [HideInInspector] public bool isCrouching = false; // 是否正在蹲下（隐藏于Inspector）
    [SerializeField] private float crouchSpeed = 3.5f; // 蹲下时的移动速度（可在Inspector中调整）
    
    // 公共属性 - 用于外部访问参数
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public float MaxVerticalSpeed => maxVerticalSpeed;
    public float DashSpeed => dashSpeed;
    public int DashDuration => dashDuration;
    public float CrouchSpeed => crouchSpeed;

    /// <summary>
    /// 初始化方法，在游戏开始时调用
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        coll = GetComponent<BoxCollider2D>(); // 获取碰撞体组件
        deathResp = GetComponent<DeathAndRespawn>(); // 获取死亡重生组件
        stop = GetComponent<StopObject>(); // 获取停止对象组件
        movementCommand = GetComponent<MovementCommand>(); // 获取运动指令组件

        gravityScale = rb.gravityScale; // 保存初始重力缩放
        canDash = true; // 初始化冲刺状态
        
        // 调试信息：显示层级设置
        Debug.Log($"PlayerMovement初始化 - Wall层级掩码: {wall.value}, 层级名称: {LayerMask.LayerToName(wall.value)}");
    }

    /// <summary>
    /// 每帧更新方法，处理输入检测
    /// </summary>
    void Update()
    {
        // 检查是否使用模拟输入
        if (movementCommand != null && movementCommand.IsExecutingCommand())
        {
            useSimulatedInput = true;
        }
        else
        {
            useSimulatedInput = false;
        }

        // 获取真实输入状态（无论是否使用模拟输入都获取，作为备用）
        //KeyState realTempKeyJump = UpdateKeyState("Jump");
        //KeyState realTempKeyDash = UpdateKeyState("Fire1");
        //KeyState realTempKeyCrouch = UpdateKeyState("Fire2");

        if (!useSimulatedInput)
        {
            // 使用真实输入
            //tempKeyJump = realTempKeyJump;
            //tempKeyDash = realTempKeyDash;
            //tempKeyCrouch = realTempKeyCrouch;
            UpdateSimulatedKeyStates();
        }
        else
        {
            // 使用模拟输入状态
            UpdateSimulatedKeyStates();
        }
    }

    /// <summary>
    /// 更新模拟按键状态
    /// </summary>
    private void UpdateSimulatedKeyStates()
    {
        if (movementCommand == null) return;

        // 根据MovementCommand的模拟输入更新按键状态
        if (movementCommand.GetSimulatedJump())
        {
            // 跳跃指令：模拟按住跳跃键，但实际跳跃动作只在开始时执行一次
            // 指令持续2秒，但跳跃动作只在第一帧触发
            if (tempKeyJump == KeyState.Off)
            {
                tempKeyJump = KeyState.Down; // 第一帧：按下跳跃键
            }
            else if (tempKeyJump == KeyState.Down)
            {
                tempKeyJump = KeyState.Held; // 后续帧：保持按住状态
            }
            // 保持Held状态直到指令结束
        }
        else
        {
            tempKeyJump = KeyState.Off;
        }

        if (movementCommand.GetSimulatedDash())
        {
            // 冲刺指令：模拟按下冲刺键，但实际冲刺动作只在开始时执行一次
            if (tempKeyDash == KeyState.Off)
            {
                tempKeyDash = KeyState.Down; // 第一帧：按下冲刺键
            }
            else if (tempKeyDash == KeyState.Down)
            {
                tempKeyDash = KeyState.Held; // 后续帧：保持按住状态
            }
            // 保持Held状态直到指令结束
        }
        else
        {
            tempKeyDash = KeyState.Off;
        }

        if (movementCommand.GetSimulatedCrouch())
        {
            // 蹲下指令：模拟按住蹲下键
            tempKeyCrouch = KeyState.Held;
        }
        else
        {
            tempKeyCrouch = KeyState.Off;
        }
    }

    /// <summary>
    /// 固定更新方法，处理物理相关的移动逻辑
    /// </summary>
    void FixedUpdate()
    {
        // 获取输入
        if (!useSimulatedInput)
        {
           // dirX = Input.GetAxisRaw("Horizontal"); // 获取水平输入轴
           // dirY = Input.GetAxisRaw("Vertical"); // 获取垂直输入轴
        }
        else
        {
            // 使用模拟输入
            if (movementCommand != null)
            {
                dirX = movementCommand.GetSimulatedDirX();
                dirY = movementCommand.GetSimulatedDirY();
            }
        }

        // 将输入状态从Update传递到FixedUpdate
        keyJump = FixedUpdateKeyState(tempKeyJump, keyJump); // 更新跳跃键状态
        keyDash = FixedUpdateKeyState(tempKeyDash, keyDash); // 更新冲刺键状态
        keyCrouch = FixedUpdateKeyState(tempKeyCrouch, keyCrouch); // 更新蹲下键状态

        // 先更新空中状态，确保跳跃检测使用正确的状态
        bool wasAirborne = isAirborne;
        isAirborne = !IsGrounded();

        if (!deathResp.dead && !stop.stopped) // 如果玩家未死亡且未被停止
        {
            UpdateFacing(); // 更新朝向
            UpdateCrouch(); // 更新蹲下状态
            DashCheck(); // 检查冲刺
            UpdateVelocity(); // 更新速度
        }
        
        // 只有在地面上且处于idle或walk状态时才恢复冲刺能力
        if (IsGrounded() && IsInWalkOrIdleState() && !canDash)
        {
            canDash = true;
            Debug.Log("在地面上进入walk/idle状态 - 恢复冲刺能力");
        }
        
        // 如果从空中落地，进入idle状态
        if (wasAirborne && !isAirborne && !isDashing)
        {
            EnterIdleState();
            Debug.Log("落地 - 进入idle状态");
            
            // 如果使用模拟输入，落地后强制重置方向输入，等待下一个指令
            if (useSimulatedInput && movementCommand != null)
            {
                // 通知MovementCommand落地事件，让它知道可以安全地重置方向
                movementCommand.OnPlayerLanded();
                Debug.Log("落地 - 通知MovementCommand重置方向输入");
            }
        }
        
        // 调试信息：显示按键状态和地面检测
        if (Time.frameCount % 60 == 0) // 每60帧输出一次
        {
            bool grounded = IsGrounded();
            bool inWalkOrIdle = IsInWalkOrIdleState();
            Debug.Log($"状态检查 - 着地: {grounded}, 可跳跃状态: {inWalkOrIdle}, 冲刺: {isDashing}, 蹲下: {isCrouching}, 垂直速度: {rb.velocity.y:F2}");
        }
    }

    /// <summary>
    /// 更新玩家速度的方法 - 处理五个基础动作
    /// </summary>
    private void UpdateVelocity()
    {
        if (!isDashing) // 如果不在冲刺状态
        {
            // 处理行走和静止动作
            float currentMoveSpeed = isCrouching ? crouchSpeed : moveSpeed; // 根据蹲下状态选择移动速度

            if (IsGrounded()) // 地面上的水平移动
            {
                rb.velocity = new Vector2(dirX * currentMoveSpeed, rb.velocity.y);
            }
            else if (dirX != 0) // 空中的水平移动
            {
                // 在空中时，根据输入方向调整水平速度
                float targetHorizontalVelocity = dirX * moveSpeed;
                float currentHorizontalVelocity = rb.velocity.x;
                
                // 平滑调整到目标速度，避免突然的速度变化
                float velocityChange = targetHorizontalVelocity - currentHorizontalVelocity;
                float maxVelocityChange = moveSpeed * 0.1f; // 每帧最大速度变化
                
                if (Mathf.Abs(velocityChange) > maxVelocityChange)
                {
                    velocityChange = Mathf.Sign(velocityChange) * maxVelocityChange;
                }
                
                float newHorizontalVelocity = currentHorizontalVelocity + velocityChange;
                rb.velocity = new Vector2(newHorizontalVelocity, rb.velocity.y);
                
                Debug.Log($"空中移动 - 输入方向: {dirX}, 目标速度: {targetHorizontalVelocity}, 当前速度: {currentHorizontalVelocity}, 新速度: {newHorizontalVelocity}");
            }
            // 如果dirX为0，保持当前水平速度，不强制停止（让重力自然影响）
        }

        // 跳跃检测 - 只能在地面上的walk或idle状态跳跃
        if (keyJump == KeyState.Down && IsInWalkOrIdleState() && !isDashing && !isCrouching)
        {
            // 跳跃时设置初始水平速度，实现有方向的跳跃
            // 注意：这里使用dirX设置初始速度，但后续的空中移动由AirMove指令控制
            float horizontalVelocity = dirX * moveSpeed;
            rb.velocity = new Vector2(horizontalVelocity, jumpForce); // 设置跳跃速度
            
            Debug.Log($"跳跃触发！方向: {dirX}, 水平速度: {horizontalVelocity}, 总速度: {rb.velocity}");
        }
        else if (keyJump == KeyState.Down)
        {
            Debug.Log($"跳跃检测失败 - keyJump: {keyJump}, IsInWalkOrIdleState: {IsInWalkOrIdleState()}, isDashing: {isDashing}, isCrouching: {isCrouching}, dirX: {dirX}");
        }
        else if (keyJump == KeyState.Down && !IsInWalkOrIdleState())
        {
            Debug.Log($"跳跃失败：只能在walk或idle状态跳跃 - 着地: {IsGrounded()}, 水平速度: {rb.velocity.x}, 垂直速度: {rb.velocity.y}");
        }
        else if (keyJump == KeyState.Down && isDashing)
        {
            Debug.Log($"跳跃失败：正在冲刺");
        }
        else if (keyJump == KeyState.Down && isCrouching)
        {
            Debug.Log($"跳跃失败：正在蹲下");
        }

        // 限制垂直速度
        if (rb.velocity.y < -maxVerticalSpeed)
        {
            rb.velocity = new Vector2(rb.velocity.x, -maxVerticalSpeed);
        }
    }

    /// <summary>
    /// 更新蹲下状态的方法
    /// </summary>
    private void UpdateCrouch()
    {
        if (keyCrouch == KeyState.Down || keyCrouch == KeyState.Held)
        {
            if (IsGrounded() && !isDashing) // 只有在地面上且不在冲刺时才能蹲下
            {
                isCrouching = true;
            }
        }
        else if (keyCrouch == KeyState.Up)
        {
            isCrouching = false;
        }
    }

    /// <summary>
    /// 检查冲刺状态的方法
    /// </summary>
    private void DashCheck()
    {
        if (keyDash == KeyState.Down && canDash && !isDashing && !isCrouching) // 蹲下时不能冲刺，但可以在空中冲刺
        {
            canDash = false; // 开始冲刺时禁用冲刺
            isDashing = true;
            dashState = dashDuration;
            
            // 处理四个方向的冲刺输入
            dashDirection = GetDashDirection();

            // 禁用重力，实现无重力冲刺
            rb.gravityScale = 0f;
            
            rb.velocity = dashDirection * dashSpeed; // 根据冲刺方向设置速度
            Debug.Log($"冲刺触发！方向: {dashDirection}, 速度: {rb.velocity}, 重力: {rb.gravityScale}");
        }
        else if (keyDash == KeyState.Down && !canDash)
        {
            Debug.Log($"冲刺失败：无法冲刺 (canDash: {canDash})");
        }
        else if (keyDash == KeyState.Down && isDashing)
        {
            Debug.Log($"冲刺失败：正在冲刺中");
        }
        else if (keyDash == KeyState.Down && isCrouching)
        {
            Debug.Log($"冲刺失败：正在蹲下");
        }

        if (isDashing) // 冲刺移动
        {
            if (dashState > 0) // 冲刺计时器检查
            {
                // 确保冲刺期间没有重力影响
                rb.gravityScale = 0f;
                
                // 如果撞到墙壁则停止冲刺
                if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.x * Vector2.right, .1f, wall)) // 检查X方向是否有墙壁
                {
                    EndDash();
                    return;
                }
                if (Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, dashDirection.y * Vector2.up, .1f, wall)) // 检查Y方向是否有墙壁
                {
                    EndDash();
                    return;
                }

                dashState--; // 更新冲刺计时器
            }
            else // 结束冲刺
            {
                EndDash();
            }
        }
    }

    /// <summary>
    /// 获取冲刺方向的方法 - 支持四个方向
    /// </summary>
    /// <returns>标准化的冲刺方向向量</returns>
    private Vector2 GetDashDirection()
    {
        Vector2 direction = new Vector2(dirX, dirY);
        
        // 如果没有输入方向，根据朝向执行水平冲刺
        if (direction == Vector2.zero)
        {
            if (facingLeft)
            {
                direction = Vector2.left;
            }
            else
            {
                direction = Vector2.right;
            }
        }
        else
        {
            // 标准化方向向量，确保四个方向的速度一致
            direction = direction.normalized;
        }
        
        return direction;
    }

    /// <summary>
    /// 结束冲刺的方法
    /// </summary>
    private void EndDash()
    {
        rb.velocity = Vector2.zero; // 冲刺结束后立即停止
        rb.gravityScale = gravityScale; // 恢复重力
        isDashing = false;
        dashState = 0;
        // 注意：不在这里恢复canDash，只有在EnterIdleState中才会恢复
        Debug.Log($"冲刺结束，当前速度: {rb.velocity}, 重力恢复: {rb.gravityScale}, canDash: {canDash}");
    }

    /// <summary>
    /// 更新朝向的方法
    /// </summary>
    private void UpdateFacing()
    {
        if (!isDashing) // 冲刺时不能更新朝向
        {
            if (dirX > 0) // 如果向右移动
            {
                facingLeft = false; // 面向右侧
            }
            else if (dirX < 0) // 如果向左移动
            {
                facingLeft = true; // 面向左侧
            }
        }
    }

    /// <summary>
    /// 检查玩家是否着地
    /// </summary>
    /// <returns>如果玩家下方有地面则返回true</returns>
    public bool IsGrounded()
    {
        // 使用OverlapBox检测地面
        Vector2 boxCenter = coll.bounds.center + Vector3.down * (coll.bounds.size.y * 0.5f + 0.05f);
        Vector2 boxSize = new Vector2(coll.bounds.size.x * 0.8f, 0.1f);
        
        // 检测所有碰撞体，不仅仅是wall层级
        Collider2D[] allHits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        
        // 过滤掉玩家自己的碰撞体
        List<Collider2D> groundHits = new List<Collider2D>();
        foreach (var hit in allHits)
        {
            if (hit.gameObject != gameObject && !hit.isTrigger)
            {
                groundHits.Add(hit);
            }
        }
        
        bool isGrounded = groundHits.Count > 0;
        
        // 调试信息 - 每次跳跃失败时都输出
        if (keyJump == KeyState.Down && !isGrounded)
        {
            string hitNames = "";
            foreach (var hit in groundHits)
            {
                hitNames += hit.name + "(" + LayerMask.LayerToName(hit.gameObject.layer) + "), ";
            }
            Debug.Log($"地面检测失败 - 检测到: {groundHits.Count} 个地面碰撞体 [{hitNames}], 结果: {isGrounded}, 玩家位置: {transform.position}");
        }
        
        return isGrounded;
    }

    /// <summary>
    /// 检查玩家是否处于walk或idle状态（可以跳跃的状态）
    /// </summary>
    /// <returns>如果玩家在地面上且处于walk或idle状态则返回true</returns>
    public bool IsInWalkOrIdleState()
    {
        // 必须在地面上
        if (!IsGrounded())
        {
            return false;
        }
        
        // 不能正在冲刺
        if (isDashing)
        {
            return false;
        }
        
        // 不能正在蹲下
        if (isCrouching)
        {
            return false;
        }
        
        // 注意：移除了垂直速度检查，因为一旦接触地面就应该可以跳跃
        // 这样玩家从上往下落到平台上时能立即跳跃
        
        // 此时玩家处于walk或idle状态
        return true;
    }

    /// <summary>
    /// 重置所有与冲刺相关的变量
    /// </summary>
    public void ResetDashAndGrab()
    {
        // 重置冲刺相关变量并进入idle状态
        EnterIdleState();
        Debug.Log("重置冲刺和抓取状态 - 进入idle状态");
    }

    /// <summary>
    /// 直接触发冲刺的方法（用于自动测试）
    /// </summary>
    /// <param name="direction">冲刺方向</param>
    public void TriggerDash(Vector2 direction)
    {
        if (canDash && !isDashing && !isCrouching)
        {
            canDash = false; // 开始冲刺时禁用冲刺
            isDashing = true;
            dashState = dashDuration;
            
            // 标准化方向向量
            dashDirection = direction.normalized;
            
            // 禁用重力，实现无重力冲刺
            rb.gravityScale = 0f;
            
            rb.velocity = dashDirection * dashSpeed; // 根据冲刺方向设置速度
            
            Debug.Log($"自动冲刺触发 - 方向: {dashDirection}, 重力: {rb.gravityScale}");
        }
    }

    /// <summary>
    /// 设置蹲下状态的方法（供外部调用）
    /// </summary>
    /// <param name="crouchState">要设置的蹲下状态</param>
    public void SetCrouchState(bool crouchState)
    {
        isCrouching = crouchState;
        Debug.Log($"蹲下状态设置为: {crouchState}");
    }

    /// <summary>
    /// 进入idle状态的方法 - 确保状态一致性
    /// 注意：只有在地面上时才会恢复冲刺能力
    /// </summary>
    public void EnterIdleState()
    {
        isDashing = false;
        dashState = 0;
        // 只有在地面上时才恢复冲刺能力
        if (IsGrounded())
        {
            canDash = true; // 恢复冲刺能力
            Debug.Log("在地面上进入idle状态 - 恢复冲刺能力");
        }
        else
        {
            Debug.Log("在空中进入idle状态 - 不恢复冲刺能力");
        }
        isCrouching = false;
        rb.gravityScale = gravityScale; // 确保重力恢复正常
        Debug.Log("进入idle状态 - 重置所有运动状态");
    }

    /// <summary>
    /// 测试地面检测的方法 - 检测所有碰撞体
    /// </summary>
    public bool TestGroundDetection()
    {
        Vector2 boxCenter = coll.bounds.center + Vector3.down * (coll.bounds.size.y * 0.5f + 0.05f);
        Vector2 boxSize = new Vector2(coll.bounds.size.x * 0.8f, 0.1f);
        
        Collider2D[] allHits = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f);
        
        Debug.Log($"测试地面检测 - 检测区域: {boxCenter}, 大小: {boxSize}");
        Debug.Log($"检测到: {allHits.Length} 个碰撞体");
        
        List<Collider2D> groundHits = new List<Collider2D>();
        foreach (var hit in allHits)
        {
            if (hit.gameObject != gameObject && !hit.isTrigger)
            {
                groundHits.Add(hit);
                Debug.Log($"地面碰撞体: {hit.name}, 层级: {LayerMask.LayerToName(hit.gameObject.layer)}, 位置: {hit.transform.position}");
            }
            else
            {
                Debug.Log($"忽略碰撞体: {hit.name} (自己或触发器)");
            }
        }
        
        Debug.Log($"有效地面碰撞体数量: {groundHits.Count}");
        return groundHits.Count > 0;
    }

    /// <summary>
    /// 更新按键状态
    /// </summary>
    /// <param name="keyName">按键名称</param>
    /// <returns>按键的当前状态</returns>
    private KeyState UpdateKeyState(string keyName)
    {
        KeyState key;

        if (Input.GetButton(keyName)) // 如果按键被按下
        {
            key = KeyState.Held; // 设置为按住状态
        }
        else // 如果按键未被按下
        {
            key = KeyState.Off; // 设置为关闭状态
        }

        return key;
    }

    /// <summary>
    /// 在FixedUpdate中更新按键状态
    /// 处理按键的按下和抬起事件
    /// </summary>
    /// <param name="tempKey">临时按键状态</param>
    /// <param name="key">当前按键状态</param>
    /// <returns>更新后的按键状态</returns>
    private KeyState FixedUpdateKeyState(KeyState tempKey, KeyState key)
    {
        /*
        按键状态转换逻辑：
        - 如果临时状态是Held（按键被按下），且当前状态是Off或Up，则转换为Down（按键刚被按下）
        - 如果临时状态是Held，且当前状态是Down或Held，则保持Held（按键持续按住）
        - 如果临时状态是Off（按键未被按下），且当前状态是Down或Held，则转换为Up（按键刚被释放）
        - 如果临时状态是Off，且当前状态是Up或Off，则保持Off（按键持续未按下）
        */

        if (tempKey == KeyState.Held) // 如果临时状态是按住
        {
            if (key == KeyState.Off || key == KeyState.Up) // 如果当前状态是关闭或抬起
            {
                key = KeyState.Down; // 设置为按下状态（按键刚被按下）
            }
            else // 如果当前状态是按下或按住
            {
                key = KeyState.Held; // 保持按住状态
            }
        }
        else if (tempKey == KeyState.Off) // 如果临时状态是关闭
        {
            if (key == KeyState.Down || key == KeyState.Held) // 如果当前状态是按下或按住
            {
                key = KeyState.Up; // 设置为抬起状态（按键刚被释放）
            }
            else // 如果当前状态是抬起或关闭
            {
                key = KeyState.Off; // 保持关闭状态
            }
        }

        return key;
    }


} 