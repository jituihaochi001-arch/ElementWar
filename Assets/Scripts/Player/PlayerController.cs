using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

public class PlayerController : SingleMonoBase<PlayerController>
{
    public PlayerModel currentModel;

    // 切换角色事件
    public event Action<PlayerModel> OnPlayerModelChanged;
    // 死亡事件
    public event Action OnDeath;

    //相机
    private Transform cameraTransform;
    [Tooltip("正常视角相机")]
    public CinemachineFreeLook freeLookCamera;
    [Tooltip("瞄准视角相机")]
    public CinemachineFreeLook aimingCamera;
    
    //玩家输入系统
    private MyInputSystem input;
    public Vector2 moveInput;
    public bool isSprint;//冲刺输入
    public bool isAiming;//瞄准输入
    public bool isJumping;//跳跃输入
    public bool isFire;//开火输入

    //瞄准相关
    
    [Tooltip("瞄准目标")]
    public Transform aimTarget;
    [Tooltip("射线检测最大距离")]
    public float maxRayDistance = 1000f;
    [Tooltip("射线检测层级")]
    public LayerMask aimLayerMask = ~0;

    //开火相机抖动
    public CinemachineImpulseSource impulseSource;
    //转向速度
    public float rotationSpeed;

    [HideInInspector]
    public Vector3 localMovement;//本地空间下玩家移动方向
    [HideInInspector]
    public Vector3 worldMovement;//世界空间下玩家移动方向
    protected override void Awake()
    {
        base.Awake();
        input = new MyInputSystem();
        
    }
    private void Start()
    {
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;//锁定光标
        ExitAim();
        impulseSource = aimingCamera.GetComponent<CinemachineImpulseSource>();
        ResetCameraTarget();
    }
    private void Update()
    {
        //更新玩家输入
        moveInput = input.Player.Move.ReadValue<Vector2>().normalized;
        isSprint = input.Player.IsSprint.IsPressed();
        isAiming = input.Player.IsAiming.IsPressed();
        isJumping = input.Player.IsJumping.triggered;
        isFire = input.Player.Fire.IsPressed();
        //计算玩家移动方向
        Vector3 cameraForwardProjection = new Vector3(cameraTransform.forward.x,0,cameraTransform.forward.z).normalized;
        //世界坐标下方向向量
        worldMovement = (cameraForwardProjection * moveInput.y + cameraTransform.right * moveInput.x).normalized;
        //本地方向向量
        localMovement = currentModel.transform.InverseTransformVector(worldMovement);

        //切换角色输入
        if (input.Player.First.triggered)
        {
            SwitchPlayerModel(0);
        }
        else if (input.Player.Second.triggered)
        {
            SwitchPlayerModel(1);
        }
    }
    //切换角色模型
    public void SwitchPlayerModel(int index)
    {
        if (index >= GameManager.Instance.playerModels.Length || GameManager.Instance.playerModels[index] == null) return;
        currentModel.Exit();
        currentModel = GameManager.Instance.playerModels[index];
        currentModel.Enter();
        ResetCameraTarget();//重置相机跟随角色
        OnPlayerModelChanged?.Invoke(currentModel);
    }
    public void EnterAim()
    {
        //同步瞄准相机和自由相机旋转角度
        aimingCamera.m_XAxis.Value = freeLookCamera.m_XAxis.Value;
        aimingCamera.m_YAxis.Value = freeLookCamera.m_YAxis.Value;

        currentModel.EnterAnim();

        //设置相机优先级
        freeLookCamera.Priority = 0;
        aimingCamera.Priority = 100;

    }
    public void ExitAim()
    {
        //同步瞄准相机和自由相机旋转角度
        freeLookCamera.m_XAxis.Value = aimingCamera.m_XAxis.Value;
        freeLookCamera.m_YAxis.Value = aimingCamera.m_YAxis.Value;

        currentModel.ExitAnim();

        //设置相机优先级
        freeLookCamera.Priority = 100;
        aimingCamera.Priority = 0;
    }
    //重置相机
    public void ResetCameraTarget()
    {
        aimingCamera.Follow = currentModel.transform;
        aimingCamera.LookAt = currentModel.transform;
        freeLookCamera.Follow = currentModel.transform;
        freeLookCamera.LookAt = currentModel.transform;
    }
   //抖动屏幕
    public void ShakeCamera()
    {
        impulseSource.GenerateImpulse();
    }
    public void DisableInput()
    {
        input.Disable();                       // 禁用输入系统
        Cursor.lockState = CursorLockMode.None; // 解锁鼠标
        Cursor.visible = true;                  // 显示光标
    }

    /// <summary>
    /// 启用玩家输入（回到游戏）
    /// </summary>
    public void EnableInput()
    {
        input.Enable();
        Cursor.lockState = CursorLockMode.Locked; // 锁定鼠标
        Cursor.visible = false;                    // 隐藏光标
    }
    private void OnEnable()
    {
        input.Enable();
    }
    private void OnDisable()
    {
        input.Disable();
    }
    public void Die()
    {
        OnDeath?.Invoke();
    }
}
