using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateBase : StateBase
{
    protected EnemyModel enemyModel;
    public override void Init(IStateMachineOwner owner)
    {     
        enemyModel = (EnemyModel)owner;
    }
    
    public override void Enter()
    {
        MonoManager.Instance.AddUpdateAction(Update);
    }

    public override void Exit()
    {
        MonoManager.Instance.RemoveUpdateAction(Update);
    } 

    public override void Update()
    {
        
    }
    public override void Destroy()
    {
       
    }
    //判断当前动画是否播放完毕
    protected bool IsAnimationBreak(int layer)
    {
        AnimatorStateInfo info = enemyModel.anim.GetCurrentAnimatorStateInfo(layer);
        return info.normalizedTime >= 1.0f && !enemyModel.anim.IsInTransition(layer);
    }
    
}
