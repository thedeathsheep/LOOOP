using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 摄像机设置控制器 - 控制摄像机的跟随和缩放功能
/// 用于实现2D游戏中的摄像机行为
/// </summary>
public class CameraSettings : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private bool enableFollowingCamera = true; // 是否启用摄像机跟随功能
    [SerializeField] private Transform player; // 玩家对象的Transform组件
    [SerializeField] private Vector3 offset = Vector3.zero; // 相机相对于玩家的偏移量
    [SerializeField] private float followSpeed = 5f; // 跟随速度（平滑跟随）
    
    [Header("缩放设置")]
    [SerializeField] private float cameraSize = 1f; // 摄像机缩放大小
    [SerializeField] private bool enableSmoothZoom = true; // 是否启用平滑缩放
    [SerializeField] private float zoomSpeed = 2f; // 缩放速度
    
    private Camera cam; // 摄像机组件引用
    private Vector3 targetPosition; // 目标位置
    private float targetSize; // 目标缩放大小

    /// <summary>
    /// 初始化方法，在游戏开始时调用
    /// </summary>
    private void Start()
    {
        cam = GetComponent<Camera>(); // 获取当前GameObject上的Camera组件
        
        // 自动查找玩家对象（如果没有手动设置）
        if (player == null)
        {
            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
            if (playerMovement != null)
            {
                player = playerMovement.transform;
                Debug.Log("CameraSettings: 自动找到玩家对象");
            }
            else
            {
                Debug.LogWarning("CameraSettings: 找不到玩家对象，请在Inspector中手动设置");
            }
        }
        
        // 初始化目标位置和缩放
        if (player != null)
        {
            targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
        }
        targetSize = 1f / cameraSize;
    }

    /// <summary>
    /// 每帧更新方法，处理摄像机的跟随和缩放逻辑
    /// </summary>
    private void Update()
    {
        if (enableFollowingCamera && player != null) // 如果启用了摄像机跟随功能且有玩家对象
        {
            UpdateCameraFollow();
        }
        
        UpdateCameraZoom();
    }
    
    /// <summary>
    /// 更新相机跟随逻辑
    /// </summary>
    private void UpdateCameraFollow()
    {
        // 计算目标位置（玩家位置 + 偏移量，保持Z轴不变）
        targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z) + offset;
        
        // 平滑跟随
        if (followSpeed > 0)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // 直接跟随（无平滑）
            transform.position = targetPosition;
        }
    }
    
    /// <summary>
    /// 更新相机缩放逻辑
    /// </summary>
    private void UpdateCameraZoom()
    {
        targetSize = 1f / cameraSize;
        
        if (enableSmoothZoom && zoomSpeed > 0)
        {
            // 平滑缩放
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, zoomSpeed * Time.deltaTime);
        }
        else
        {
            // 直接缩放
            cam.orthographicSize = targetSize;
        }
    }
    
    /// <summary>
    /// 设置相机缩放（供外部调用）
    /// </summary>
    /// <param name="newSize">新的缩放值</param>
    public void SetCameraSize(float newSize)
    {
        cameraSize = newSize;
    }
    
    /// <summary>
    /// 设置相机偏移量（供外部调用）
    /// </summary>
    /// <param name="newOffset">新的偏移量</param>
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
}
