using System.Collections;
using UnityEngine;

public class ZombieAttackState : EnemyStateBase
{
    private bool _isAttacking = false;  // 是否正在攻击（动画播放中）

    public override void Enter()
    {
        base.Enter();
        // 进入攻击状态时，尝试攻击一次
        TryAttack();
    }

    public override void Update()
    {
        base.Update();

        // 如果目标不在范围内，切回 Move
        if (!enemyModel.IsAttackTargetInAttackRange())
        {
            enemyModel.SwitchState(EnemyState.Move);
            return;
        }

        // 如果正在攻击（动画播放中），等它播完
        if (_isAttacking) return;

        // 如果冷却好了，再攻击一次
        if (Time.time - enemyModel.lastAttackTime >= enemyModel.attackCooldown)
        {
            TryAttack();
        }
    }

    /// <summary>
    /// 尝试发动一次攻击
    /// </summary>
    private void TryAttack()
    {
        // 冷却没到，不攻击
        if (Time.time - enemyModel.lastAttackTime < enemyModel.attackCooldown)
            return;

        // 没有目标，不攻击
        if (enemyModel.attackTarget == null)
            return;

        // 记录攻击时间
        enemyModel.lastAttackTime = Time.time;
        _isAttacking = true;

        // 播放攻击动画
        enemyModel.PlayerStateAnimation("Attack");

        // 启动攻击判定协程
        enemyModel.StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// 攻击判定协程
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        // 1. 攻击前摇（等动画播到伤害判定帧）
        yield return new WaitForSeconds(0.3f);  // 根据动画时长调整

        // 2. 伤害判定：此刻目标是否还在范围内
        if (enemyModel.attackTarget != null && enemyModel.IsAttackTargetInAttackRange())
        {
            enemyModel.attackTarget.TakeDamage(enemyModel.damage);
        }

        // 3. 攻击后摇（等动画播完）
        yield return new WaitForSeconds(0.3f);  // 根据动画时长调整

        // 4. 攻击结束
        _isAttacking = false;
    }

    public override void Exit()
    {
        base.Exit();
        _isAttacking = false;
        enemyModel.StopAllCoroutines();  // 停止协程，防止退出后还在跑
    }
}