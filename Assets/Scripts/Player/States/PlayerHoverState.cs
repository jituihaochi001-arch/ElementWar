using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHoverState : PlayerStateBase
{
    public override void Enter()
    {
        base.Enter();
        playerModel.PlayerStateAnimation("Hover");
    }
    public override void Update()
    {
        base.Update();
        if (playerModel.cc.isGrounded)
            playerModel.SwitchState(PlayerState.Idle);
    }
}
