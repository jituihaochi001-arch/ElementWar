using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private int moveBlendHash;//属性
    private float moveBlend;//参数
    private float runThreshold = 0;//奔跑阈值
    private float sprintThreshold = 1;//冲刺阈值
    private float transitionSpeed = 5;//过渡速度
    public override void Init(IStateMachineOwner owner)
    {
        base.Init(owner);
        moveBlendHash = Animator.StringToHash("MoveBlend");
    }
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayerStateAnimation("Move");
    }
    public override  void Update()
    {
        base.Update();
        //若当前控制该角色
        if (IsBeControl())
        {
            //跳跃状态监听
            if (playerController.isJumping)
            {
                SwitchToHover();
                return;
            }
            //待机状态监听
            if (playerController.moveInput.magnitude == 0)
            {
                playerModel.SwitchState(PlayerState.Idle);
                return;
            }
            //冲刺状态监听
            if (playerController.isSprint)
            {
                moveBlend = Mathf.Lerp(moveBlend , sprintThreshold , transitionSpeed*Time.deltaTime);
            }
            else
            {
                moveBlend = Mathf.Lerp(moveBlend, runThreshold, transitionSpeed * Time.deltaTime);
            }
            playerModel.anim.SetFloat(moveBlendHash,moveBlend);
            //处理方向
            //计算本地空间移动方向与模型正前方之间的夹角
            float rad = Mathf.Atan2(playerController.localMovement.x,playerController.localMovement.z);
           
            //旋转到移动方向
            playerModel.transform.Rotate(0,rad * playerController.rotationSpeed * Time.deltaTime,0);
        }
        //若为其他队友/人机模式,自动跟随玩家角色
        else
        {
            //处理移动速度
            if(playerModel.DistanceOfCurrentPlayerModel()-playerModel.stoppingDistance < 2f)
            {
                moveBlend = Mathf.Lerp(moveBlend,runThreshold,transitionSpeed * Time.deltaTime);
            }
            else
            {
                moveBlend = Mathf.Lerp(moveBlend, sprintThreshold, transitionSpeed * Time.deltaTime);
            }
            playerModel.anim.SetFloat (moveBlendHash,moveBlend);
            //若距离小于停止距离，则角色停止移动，停在玩家角色旁边
            if (playerModel.DistanceOfCurrentPlayerModel() <= playerModel.stoppingDistance)
            {
                playerModel.SwitchState(PlayerState.Idle);
            }
            playerModel.navMeshAgent.SetDestination(playerController.currentModel.transform.position);
        }
    }
}
