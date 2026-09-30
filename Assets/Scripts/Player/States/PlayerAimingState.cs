using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAimingState : PlayerStateBase
{
    private int aimingXHash;
    private int aimingYHash;
    private float aimingX = 0;
    private float aimingY = 0;
    private float transitionSpeed = 5f;

    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        aimingXHash = Animator.StringToHash("AimingX");
        aimingYHash = Animator.StringToHash("AimingY");
    }
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayerStateAnimation("Aiming");
        if (IsBeControl())
        {
            
            UpdateAimingTarget();
            playerController.EnterAim();
        }
    }
    public override void Update()
    {
        base.Update();
        if (IsBeControl())
        {
            //让模型转向至相机方向
            playerModel.transform.rotation = Quaternion.Euler(0,Camera.main.transform.rotation.eulerAngles.y,0);
            UpdateAimingTarget();
            //退出瞄准状态监听
            if (!playerController.isAiming && !playerController.isFire)
                playerModel.SwitchState(PlayerState.Idle);
            //开火监听
            if (playerController.isFire)
            {
                playerModel.weapon.Fire(playerController.aimTarget.position);
                playerController.ShakeCamera();//开火时相机抖动
            }
            //处理移动输入
            aimingX = Mathf.Lerp(aimingX, playerController.moveInput.x, transitionSpeed * Time.deltaTime);
            aimingY = Mathf.Lerp(aimingY, playerController.moveInput.y, transitionSpeed * Time.deltaTime);
            playerModel.anim.SetFloat(aimingXHash, aimingX);
            playerModel.anim.SetFloat(aimingYHash, aimingY);
        }
    }
 
    public override void Exit()
    {
        base.Exit();
        if (IsBeControl())
        {
            playerController.ExitAim();
        }
    }
    //从屏幕中心发射射线确认瞄准位置
    private void UpdateAimingTarget()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;
        // 排除角色自己所在的 Layer
        int layerMask = playerController.aimLayerMask;
        layerMask &= ~(1 << LayerMask.NameToLayer("Player"));

        //若射线击中物体
        if (Physics.Raycast(ray, out hit,playerController.maxRayDistance,layerMask))
        {
            //更新瞄准目标的位置
            playerController.aimTarget.position = hit.point;
          
        }
        else
        {           
            playerController.aimTarget.position = ray.origin + ray.direction * 1000f;         
        }
    }
}
