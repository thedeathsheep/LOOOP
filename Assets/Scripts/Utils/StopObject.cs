using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 停止对象控制器 - 控制玩家在屏幕转换时的停止和恢复逻辑
/// 处理玩家在屏幕切换时的暂停和重生点更新
/// </summary>
public class StopObject : MonoBehaviour
{
    public bool stopped = false; // 是否处于停止状态
    private int frameDuration; // 停止持续时间（帧数）
    private int timer = 0; // 计时器

    private Vector2 storedVelocity; // 存储的速度

    private bool upperTransition; // 是否为向上转换
    private GameObject transitionCamera; // 转换摄像机

    private Rigidbody2D rb; // 刚体组件
    private PlayerMovement playerMovement; // 玩家移动组件
    private DeathAndRespawn deathResp; // 死亡重生组件

    /// <summary>
    /// 初始化方法，在游戏开始时调用
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        playerMovement = GetComponent<PlayerMovement>(); // 获取玩家移动组件
        deathResp = GetComponent<DeathAndRespawn>(); // 获取死亡重生组件
    }

    /// <summary>
    /// 固定更新方法，处理停止状态的逻辑
    /// </summary>
    private void FixedUpdate()
    {
        if (stopped) // 如果处于停止状态
        {
            if (timer < frameDuration) // 如果计时器未达到持续时间
            {
                timer++; // 计时器递增
            }
            else // 如果停止时间结束
            {
                timer = 0; // 重置计时器
                stopped = false; // 停止停止状态

                // 检查组件是否为空
                if (rb == null || playerMovement == null || deathResp == null)
                {
                    Debug.LogWarning("StopObject: 缺少必要的组件，无法恢复玩家状态");
                    return;
                }

                if (upperTransition && transitionCamera != null && transitionCamera.activeInHierarchy && deathResp.dead == false) // 如果是向上转换且摄像机激活且玩家未死亡
                {
                    storedVelocity = new Vector2(0f, 11f); // 设置向上的速度
                    playerMovement.ResetDashAndGrab(); // 重置冲刺状态
                }

                rb.velocity = storedVelocity; // 恢复存储的速度
                rb.gravityScale = playerMovement.gravityScale; // 恢复重力缩放
                playerMovement.canDash = true; // 恢复冲刺次数

                // 更新重生点
                List<GameObject> spawnpointsInCamera = new List<GameObject>(); // 摄像机内的重生点列表

                // 只选择摄像机内的重生点
                foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Respawn")) // 遍历所有重生点
                {
                    if (VisibleInCamera(obj)) // 如果重生点在摄像机内可见
                    {
                        spawnpointsInCamera.Add(obj); // 添加到列表
                    }
                }

                // 将最近的重生点设置为新的重生点
                if (spawnpointsInCamera.Count > 0)
                {
                    deathResp.respawnPosition = deathResp.Nearest(spawnpointsInCamera.ToArray()); // 找到最近的重生点
                }
            }
        }
    }

    /// <summary>
    /// 停止玩家的方法
    /// </summary>
    /// <param name="timeDuration">停止持续时间</param>
    /// <param name="up">是否为向上转换</param>
    /// <param name="camera">转换摄像机</param>
    public void Stop(float timeDuration, bool up, GameObject camera)
    {
        // 检查参数是否有效
        if (timeDuration <= 0f)
        {
            Debug.LogWarning("StopObject.Stop: 时间持续时间必须大于0");
            return;
        }

        if (camera == null)
        {
            Debug.LogWarning("StopObject.Stop: 摄像机参数不能为空");
            return;
        }

        // 检查必要的组件
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogError("StopObject.Stop: 无法获取 Rigidbody2D 组件");
                return;
            }
        }

        frameDuration = (int)(timeDuration / Time.deltaTime); // 计算停止帧数
        upperTransition = up; // 设置转换方向
        transitionCamera = camera; // 设置转换摄像机

        stopped = true; // 激活停止状态
        timer = 0; // 重置计时器

        storedVelocity = rb.velocity; // 存储当前速度
        rb.velocity = Vector2.zero; // 停止移动
        rb.gravityScale = 0f; // 停止重力
    }

    /// <summary>
    /// 检查对象是否在摄像机内可见
    /// </summary>
    /// <param name="gameObject">要检查的游戏对象</param>
    /// <returns>如果对象在摄像机内可见则返回true</returns>
    private bool VisibleInCamera(GameObject gameObject)
    {
        // 检查参数是否为空
        if (gameObject == null)
        {
            return false;
        }

        // 检查转换摄像机是否为空
        if (transitionCamera == null || transitionCamera.transform == null || transitionCamera.transform.parent == null)
        {
            return false;
        }

        // 获取 SpawnpointInitialization 组件
        SpawnpointInitialization spawnpointInit = gameObject.GetComponent<SpawnpointInitialization>();
        if (spawnpointInit == null || spawnpointInit.screen == null)
        {
            return false;
        }

        // 检查对象的屏幕是否与转换摄像机相同
        if (GameObject.ReferenceEquals(spawnpointInit.screen, transitionCamera.transform.parent.gameObject))
        {
            return true; // 在摄像机内可见
        }
        else
        {
            return false; // 不在摄像机内可见
        }
    }
}
