using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡与重生控制器 - 处理玩家的死亡动画和重生逻辑
/// 包括死亡效果、重生位置计算和状态重置
/// </summary>
public class DeathAndRespawn : MonoBehaviour
{
    private GameObject[] spawnPoints; // 所有重生点对象的数组
    [SerializeField] private GameObject Ball; // 死亡动画球体预制体（可在Inspector中设置）
    private Rigidbody2D rb; // 玩家的刚体组件
    public Vector2 respawnPosition = Vector2.zero; // 重生位置
    [SerializeField] private float deadSpeed = 5f; // 死亡时的弹飞速度

    public bool dead = false; // 死亡状态标志
    private int deathAnimationTimer = 0; // 死亡动画计时器

    /// <summary>
    /// 初始化方法，在游戏开始时调用
    /// </summary>
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 获取刚体组件
        // 查找所有重生点并找到最近的作为初始重生位置
        spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        
        if (spawnPoints.Length > 0)
        {
            respawnPosition = Nearest(spawnPoints);
        }
        else
        {
            // 如果没有找到重生点，使用玩家当前位置作为默认重生点
            respawnPosition = transform.position;
            Debug.LogWarning("DeathAndRespawn: 没有找到重生点，使用玩家当前位置作为默认重生点");
        }
    }

    /// <summary>
    /// 固定更新方法，处理死亡动画和重生逻辑
    /// </summary>
    private void FixedUpdate()
    {
        if (dead) // 如果玩家处于死亡状态
        {
            rb.velocity *= 0.9f; // 逐渐减速，使动画更流畅

            // 死亡动画：创建死亡球体效果
            if (deathAnimationTimer == 6) // 在第6帧创建死亡球体
            {

            }

            // 在球体动画结束后隐藏精灵
            if (deathAnimationTimer > 15)
            {
                GetComponent<SpriteRenderer>().enabled = false; // 隐藏精灵渲染器
            }
            else
            {
                GetComponent<SpriteRenderer>().enabled = true; // 显示精灵渲染器
            }

            // 重生逻辑
            if (deathAnimationTimer == 30) // 在第30帧执行重生
            {
                transform.position = respawnPosition; // 将玩家传送到重生点
                transform.localScale = 1f * Vector3.one; // 恢复正常大小
                rb.velocity = Vector2.zero; // 停止所有速度
                GetComponent<PlayerMovement>().canDash = true; // 重置冲刺次数
                GetComponent<PlayerMovement>().ResetDashAndGrab(); // 重置冲刺状态
                GetComponent<Animator>().SetBool("dead", false); // 关闭死亡动画状态
                GetComponent<BoxCollider2D>().enabled = true; // 重新激活碰撞体

         
                // 重置摄像机
                GetComponent<InitializeActiveCamera>().Start();
            }

            // 计时器和死亡结束
            if (deathAnimationTimer < 50) // 死亡动画总时长50帧
            {
                deathAnimationTimer++; // 计时器递增
            }
            else // 死亡动画结束
            {
                dead = false; // 重置死亡状态
                deathAnimationTimer = 0; // 重置计时器
                GetComponent<PlayerMovement>().ResetDashAndGrab(); // 停止冲刺状态
                // 重新激活玩家的渲染
                GetComponent<SpriteRenderer>().enabled = true;
            }
        }
    }

    /// <summary>
    /// 碰撞检测方法，处理与尖刺的碰撞
    /// </summary>
    /// <param name="collision">碰撞信息</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike")) // 检查是否与尖刺碰撞
        {
            dead = true; // 触发死亡状态
            rb.gravityScale = 0f; // 停止重力影响

            // 设置死亡时的弹飞速度和方向
            rb.velocity = deadSpeed * Vector2.one; // 初始弹飞速度
            if (collision.transform.position.y > transform.position.y) // 如果尖刺在玩家上方，向下弹飞
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y);
            }
            if (!GetComponent<PlayerMovement>().facingLeft) // 根据玩家朝向调整水平弹飞方向
            {
                rb.velocity = new Vector2(-rb.velocity.x, rb.velocity.y);
            }
            GetComponent<Collider2D>().enabled = false; // 禁用碰撞体
            GetComponent<Animator>().SetBool("dead", true); // 激活死亡动画状态
        }
    }

    /// <summary>
    /// 查找距离玩家最近的重生点
    /// </summary>
    /// <param name="gameObjectList">重生点对象数组</param>
    /// <returns>最近重生点的位置</returns>
    public Vector2 Nearest(GameObject[] gameObjectList)
    {
        // 检查数组是否为空
        if (gameObjectList == null || gameObjectList.Length == 0)
        {
            Debug.LogWarning("Nearest: 重生点数组为空，返回玩家当前位置");
            return transform.position;
        }

        int index = 0; // 最近对象的索引
        float minDist = Mathf.Infinity; // 最小距离，初始化为无穷大

        for (int i = 0; i < gameObjectList.Length; i++) // 遍历所有重生点
        {
            if (gameObjectList[i] == null) continue; // 跳过空对象
            
            float dist = Vector2.Distance(transform.position, gameObjectList[i].transform.position); // 计算距离

            if (dist < minDist) // 如果找到更近的重生点
            {
                index = i; // 更新索引
                minDist = dist; // 更新最小距离
            }
        }

        return gameObjectList[index].transform.position; // 返回最近重生点的位置
    }
}
