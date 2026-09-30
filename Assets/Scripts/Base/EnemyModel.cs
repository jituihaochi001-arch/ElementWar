using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Idle,
    Move,
    Attack,
    Dead
}
//敌人基类脚本
public abstract class EnemyModel : MonoBehaviour,IStateMachineOwner//标记为状态机宿主
{
    [HideInInspector]
    public Animator anim;
    protected StateMachine stateMachine;

    //寻路相关
    [HideInInspector]
    public NavMeshAgent navMeshAgent;//寻路代理

    [Tooltip("转向速度")]
    public float rotationSpeed = 300f;//转向速度
    [Tooltip("最小攻击距离")]
    public float minAttacktDistance = 1f;
    public PlayerModel attackTarget;//追击玩家目标

    //血条相关
    [Tooltip("生命值")]
    public int health = 100;
    private float currentHealth;
    private bool isDead = false;
    [Tooltip("血条预制体")]
    public GameObject healthBarPrefab;
    [Tooltip("血条生成位置")]
    public Transform healthBarPos;
    public GameObject healthBar;//实例化后的血条
    [Tooltip("血条框显示时间")]
    public float healthBarShowTime = 6f;
    private float healthBarShowTimer;

    [Tooltip("喷溅特效")]
    public GameObject bloodSmashPrefab;
    [Tooltip("滴血特效")]
    public GameObject bloodDrippingPrefab;

    [Header("攻击相关")]
    public int damage = 5;
    [Tooltip("攻击冷却时间")]
    public float attackCooldown = 2f;

    [HideInInspector]
    public float lastAttackTime = -999f;  // 上次攻击时间


    //受击相关
    protected int hitHash;
    protected int moveSpeedHash;
    protected float normalMoveSpeed = 1;
    protected float slowMoveSpeed = 0.5f;
    protected Coroutine recoverSpeedCoroutine;//恢复速度协程

    protected virtual void Awake()
    {
        stateMachine = new StateMachine(this);
        anim = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = minAttacktDistance;
        navMeshAgent.angularSpeed = rotationSpeed;
        hitHash = Animator.StringToHash("Hit");
        moveSpeedHash = Animator.StringToHash("MoveSpeed");
        currentHealth = health;
        healthBarShowTimer = healthBarShowTime;
    }
    protected virtual void Start()
    {
        SwitchState(EnemyState.Idle);
        FindAttackTarget();
        //实例化血条框
        healthBar = Instantiate(healthBarPrefab,healthBarPos.position,Quaternion.identity);
        healthBar.transform.SetParent(UIManager.Instance.WorldSpaceCanvas.transform);
    }
    protected virtual void Update()
    {   
        if(isDead) return;
        //血条相关
        if(healthBarShowTimer < healthBarShowTime)
        {
            healthBar.SetActive(true);
            healthBar.transform.position = healthBarPos.position;
            healthBarShowTimer += Time.deltaTime;
        }
        else
        {
            healthBar.SetActive(false);
        }
    }
    public virtual void FindAttackTarget()
    {
        //在玩家角色中选一个距离最近的为攻击目标
        PlayerModel[] playerModels = GameManager.Instance.playerModels;
        if (playerModels != null && playerModels.Length > 0) 
        {
            PlayerModel closePlayer = null;
            float minDistance = float.MaxValue;
            foreach(PlayerModel player in playerModels)
            {
                float distance = Vector3.Distance(transform.position ,player.transform.position);
                if(distance < minDistance)
                {
                    minDistance = distance;
                    closePlayer = player;
                }
            }
            attackTarget = closePlayer;
        }
    }
    //判断是否存在攻击对象
    public virtual bool HasAttackTarget()
    {
        return attackTarget != null;
    }
    //
    public virtual bool IsAttackTargetInAttackRange()
    {
        if (HasAttackTarget())
        {
            return Vector3.Distance(transform.position,attackTarget.transform.position) < minAttacktDistance;
        }
        return false;
    }
    //追击目标
    public virtual void ChaseTarget()
    {
        if (HasAttackTarget())
        {
            navMeshAgent.SetDestination(attackTarget.transform.position);//设置追击目标
            
        }
    }
    //减慢移速
    protected virtual void SlowMoveAnimation()
    {
        anim.SetFloat(moveSpeedHash, slowMoveSpeed);
        if(recoverSpeedCoroutine != null) 
        { 
            StopCoroutine(recoverSpeedCoroutine);
        }
        recoverSpeedCoroutine = StartCoroutine(RecoverMoveSpeed(0.5f));
    }
    protected IEnumerator RecoverMoveSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);
        //等待delay后恢复正常移速
        anim.SetFloat(moveSpeedHash, normalMoveSpeed);
        recoverSpeedCoroutine = null;
    }
    //敌人受击方法
    public virtual void Hurt(PlayerWeaponBullet bullet,float damageMultiplier = 1f)
    {
        //受击动画相关
        anim.SetTrigger(hitHash);
        SlowMoveAnimation();
        //生成喷血特效
        Vector3 bulletDirection = bullet.transform.forward;
        Quaternion rotation = Quaternion.LookRotation(-bulletDirection);
        Destroy(Instantiate(bloodSmashPrefab,bullet.transform.position,rotation),3);
        //生成滴血特效
        Destroy(Instantiate(bloodDrippingPrefab,bullet.transform.position + Vector3.up * 0.1f,Quaternion.Euler(0,0,0)),3);
        //血条相关   
        currentHealth -= bullet.damage * damageMultiplier;//基础伤害*倍率
        if (currentHealth > 0 )
        {           
            healthBarShowTimer = 0;
            healthBar.GetComponent<EnemyHealthUI>().UpdateHealthBar(currentHealth / health);
        }
        else
        {
            SwitchState(EnemyState.Dead);
            navMeshAgent.enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            currentHealth = 0;
            isDead = true;
            Destroy(healthBar);
        }           
    }
    public abstract void SwitchState(EnemyState enemyState);

    //播放动画
    public void PlayerStateAnimation(string animationName, float transition = 0.25f, int layer = 0)
    {
        anim.CrossFadeInFixedTime(animationName, transition, layer);
    }
    public void Clear()
    {
        stateMachine.Stop();
        Destroy(gameObject);
    }
}
