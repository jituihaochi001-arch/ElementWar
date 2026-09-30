using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStateBase : StateBase
{
    protected PlayerController playerController;
    protected PlayerModel playerModel;//当前状态角色模型
    public override void Init(IStateMachineOwner owner)
    {
        playerController = PlayerController.Instance;
        playerModel = (PlayerModel)owner;
    }
   
    public override void Enter()
    {
        MonoManager.Instance.AddUpdateAction(Update);//一旦进入新状态，就将update方法订阅任务
    }
    public override void Exit()
    {
        MonoManager.Instance.RemoveUpdateAction(Update);//退出状态，取消订阅任务
    }
    public override void Destroy()
    {

    }
    public override void Update()
    {
        #region 重力计算
        if (!playerModel.cc.isGrounded)//模型不在地面上时
        {
            playerModel.verticalSpeed += playerModel.gravity * Time.deltaTime;//施加重力
            if(playerModel.IsHover())//若悬空高度大于0.2，处于悬空状态
                playerModel.SwitchState(PlayerState.Hover);
        }
        else
            playerModel.verticalSpeed = playerModel.gravity * Time.deltaTime;//重置速度

        //瞄准状态监听
        if ( IsBeControl() && (playerController.isAiming || playerController.isFire))
        {
            playerModel.SwitchState(PlayerState.Aiming);
        }
        #endregion
    }
    //当前模型是否为玩家控制
    public bool IsBeControl()
    {
        return playerModel == playerController.currentModel;
    }
    public void SwitchToHover()
    {
        //计算跳跃力度
        playerModel.verticalSpeed = MathF.Sqrt(-2 * playerModel.gravity * playerModel.jumpHeight);
        //切换悬空状态
        playerModel.SwitchState(PlayerState.Hover);
    }
}
