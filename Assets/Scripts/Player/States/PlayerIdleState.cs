using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayerStateAnimation("Idle");
    }
    public override void Update()
    {
        base.Update();
        if (IsBeControl())
        {   //¼ì²âÊÇ·ñÓÐÍæ¼ÒÊäÈë
            if (playerController.moveInput.magnitude != 0)
            {
                playerModel.SwitchState(PlayerState.Move);
                return;
            }
            //¼ì²âÌøÔ¾
            if(playerController.isJumping)
            {
                SwitchToHover();
            }
        }
        else
        {
            if (playerModel.DistanceOfCurrentPlayerModel() > playerModel.stoppingDistance)
            {
                playerModel.SwitchState(PlayerState.Move);
            }
        }
    }
}
