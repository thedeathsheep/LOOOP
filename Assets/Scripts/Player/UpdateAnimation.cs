using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画更新控制器 - 控制玩家的基本动画状态
/// 根据玩家的移动状态更新动画
/// </summary>
public class UpdateAnimation : MonoBehaviour
{
    private Rigidbody2D rb; // 刚体组件
    private Animator anim; // 动画控制器组件
    private SpriteRenderer sprite; // 精灵渲染器组件

    private enum AnimationState { idle, walk, jump, fall, dash, crouch }; // 动画状态枚举：待机、行走、跳跃、下落、冲刺、蹲下
    private AnimationState state; // 当前动画状态

    [SerializeField] private PlayerMovement playerMovement; // 玩家移动组件引用（可在Inspector中设置）
    private MovementCommand movementCommand; // 运动指令组件

    /// <summary>
    /// 初始化方法，在游戏开始时调用
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        anim = GetComponent<Animator>(); // 获取动画控制器组件
        sprite = GetComponent<SpriteRenderer>(); // 获取精灵渲染器组件
        
        // 如果没有手动设置playerMovement，自动获取
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
        }
        
        // 获取MovementCommand组件
        movementCommand = GetComponent<MovementCommand>();
        
        // 验证组件设置
        ValidateComponents();
    }

    /// <summary>
    /// 验证组件设置
    /// </summary>
    private void ValidateComponents()
    {
        if (rb == null)
        {
            Debug.LogError("UpdateAnimation: 缺少 Rigidbody2D 组件！");
        }
        
        if (anim == null)
        {
            Debug.LogError("UpdateAnimation: 缺少 Animator 组件！");
        }
        else if (anim.runtimeAnimatorController == null)
        {
            Debug.LogError("UpdateAnimation: Animator 组件没有分配动画控制器！");
        }
        else
        {
            Debug.Log($"UpdateAnimation: 动画控制器已设置: {anim.runtimeAnimatorController.name}");
            
            // 检查动画参数
            if (HasParameter("state", anim))
            {
                Debug.Log("UpdateAnimation: 'state' 参数存在，动画系统准备就绪");
            }
            else
            {
                Debug.LogError("UpdateAnimation: 动画控制器缺少 'state' 参数！");
                Debug.LogError("请确保动画控制器包含名为 'state' 的整数参数");
            }
        }
        
        if (playerMovement == null)
        {
            Debug.LogError("UpdateAnimation: 缺少 PlayerMovement 组件！");
        }
        
        if (sprite == null)
        {
            Debug.LogWarning("UpdateAnimation: 缺少 SpriteRenderer 组件！");
        }
    }

    /// <summary>
    /// 固定更新方法，处理动画状态更新
    /// </summary>
    void FixedUpdate()
    {
        // 检查组件引用
        if (playerMovement == null || rb == null || anim == null)
        {
            if (Time.frameCount % 60 == 0) // 每60帧输出一次错误信息
            {
                Debug.LogWarning($"UpdateAnimation: 缺少必要组件 - playerMovement: {playerMovement != null}, rb: {rb != null}, anim: {anim != null}");
            }
            return;
        }
        
        // 检查动画控制器是否有state参数
        if (!HasParameter("state", anim))
        {
            if (Time.frameCount % 60 == 0) // 每60帧输出一次错误信息
            {
                Debug.LogError($"UpdateAnimation: 动画控制器缺少 'state' 参数！请检查动画控制器设置。");
                Debug.LogError($"当前动画控制器: {anim.runtimeAnimatorController?.name ?? "null"}");
            }
            return;
        }
        
        // 朝向翻转
        if (playerMovement.facingLeft) // 如果面向左侧
        {
            transform.localScale = new Vector2(-1, transform.localScale.y); // 水平翻转
        }
        else // 如果面向右侧
        {
            transform.localScale = new Vector2(1, transform.localScale.y); // 正常朝向
        }

        // 更新动画状态
        UpdateAnimationState();

        // 安全地设置动画参数
        try
        {
            anim.SetInteger("state", (int)state); // 将动画状态传递给动画控制器
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UpdateAnimation: 设置动画参数时出错: {e.Message}");
        }
        
        // 调试信息
        if (Time.frameCount % 60 == 0) // 每60帧输出一次
        {
            Debug.Log($"动画状态: {state}, 速度: {rb.velocity}, 着地: {playerMovement.IsGrounded()}, 蹲下: {playerMovement.isCrouching}, 冲刺: {playerMovement.isDashing}");
        }
    }

    /// <summary>
    /// 更新动画状态的方法
    /// </summary>
    private void UpdateAnimationState()
    {
        // 检查是否正在执行指令
        bool isExecutingCommand = movementCommand != null && movementCommand.IsExecutingCommand();

        if (playerMovement.isDashing) // 如果正在冲刺
        {
            state = AnimationState.dash; // 设置为冲刺状态
        }
        else if (playerMovement.isCrouching && playerMovement.IsGrounded()) // 如果正在蹲下且着地
        {
            state = AnimationState.crouch; // 设置为蹲下状态
        }
        else if (rb.velocity.y > .1f) // 如果向上移动
        {
            state = AnimationState.jump; // 设置为跳跃状态
        }
        else if (rb.velocity.y < -.1f) // 如果向下移动
        {
            state = AnimationState.fall; // 设置为下落状态
        }
        else if (playerMovement.IsGrounded()) // 如果着地
        {
            if (rb.velocity.x != 0) // 如果有水平移动
            {
                state = AnimationState.walk; // 设置为行走状态
            }
            else // 如果静止不动
            {
                state = AnimationState.idle; // 设置为待机状态
            }
        }
        else
        {
            // 空中状态，根据水平速度判断
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                state = AnimationState.walk; // 空中移动
            }
            else
            {
                state = AnimationState.idle; // 空中静止
            }
        }

        // 如果正在执行指令，添加额外的调试信息
        if (isExecutingCommand && Time.frameCount % 30 == 0)
        {
            Debug.Log($"指令执行中 - 动画状态: {state}");
        }
    }

    /// <summary>
    /// 检查动画控制器是否有指定参数
    /// </summary>
    /// <param name="paramName">参数名称</param>
    /// <param name="animator">动画控制器</param>
    /// <returns>如果参数存在则返回true</returns>
    private bool HasParameter(string paramName, Animator animator)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
        {
            return false;
        }

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
    }
}
