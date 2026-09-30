using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMoveState : EnemyStateBase
{
    public override void Enter()
    {
        base.Enter();
        enemyModel.PlayerStateAnimation("Move");
        
    }
    public override void Update()
    {
        base.Update();
        if (!enemyModel.IsAttackTargetInAttackRange())
        {
            enemyModel.ChaseTarget();
         
        }
        else
        {
            //ÈôÔÚ¹¥»÷·¶Î§ÄÚ£¬¹¥»÷Íæ¼Ò
            enemyModel.SwitchState(EnemyState.Attack);
        }
    }
}
