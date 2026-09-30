using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public enum PlayerState
{
    Idle,
    Move,
    Hover,
    Aiming
}
public class PlayerModel : MonoBehaviour,IStateMachineOwner//标记为状态机宿主
{
    [HideInInspector]
    private StateMachine stateMachine;//动画状态机
    [HideInInspector]
    public Animator anim;
    [HideInInspector]
    public CharacterController cc;
    private PlayerState currentState;//当前状态

    [Header("血量")]
    [Tooltip("最大血量")]
    public int maxHealth = 100;
    [Tooltip("当前血量")]
    public int currentHealth;
    [Tooltip("未受伤后多少秒开始回血")]
    public float regenDelay = 5f;
    [Tooltip("每次回血量")]
    public int regenAmount = 2;

    // 回血计时器
    private float _lastDamageTime;   // 上次受伤时间

    // 血量变化事件（UI 监听用）
    public event Action<int, int> OnHealthChanged;  // (当前血量, 最大血量)
   

    //约束相关
    public TwoBoneIKConstraint rightHandConstraint;//正常状态下右手约束
    public MultiAimConstraint rightHandAimConstraint;//瞄准时右手约束 
    public MultiAimConstraint bodyAimConstraint;//身体约束

    [Tooltip("重力")]
    public float gravity = -15f;
    [Tooltip("跳跃高度")]
    public float jumpHeight = 1.5f;
    public float verticalSpeed;//垂直方向速度
    public float fallHeight = 0.2f;//超过此高度，进入悬空状态

    [Tooltip("角色武器")]
    public PlayerWeapon weapon;

    //玩家在地面时前三帧速度的缓存
    private static readonly int CACHE_SIZE = 3;//三帧
    Vector3[] speedCache = new Vector3[CACHE_SIZE];//动画前三帧的玩家速度
    private int speedCache_index = 0;//缓存保存的位置
    private Vector3 averageDeltaMovement;//平均速度

    //人机相关
    [HideInInspector]
    public NavMeshAgent navMeshAgent;
    public float stoppingDistance = 2f;
   
    private void Awake()
    {
        stateMachine = new StateMachine(this);
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        currentHealth = maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = stoppingDistance;
        navMeshAgent.angularSpeed = PlayerController.Instance.rotationSpeed;
    }
    private void Start()
    {
        ExitAnim();
        SwitchState(PlayerState.Idle);//初始默认待机动画
    }
    private void Update()
    {
        // 回血逻辑：距离上次受伤超过 regenDelay 秒，且血量没满
        if (Time.time - _lastDamageTime >= regenDelay && currentHealth < maxHealth)
        {
            Heal(regenAmount);
            _lastDamageTime = Time.time;  // 重置计时，下次回血再等 5 秒
        }
    }
    //进入模型
    public void Enter()
    {
        navMeshAgent.enabled = false;
    }
    //退出模型
    public void Exit()
    {
        navMeshAgent.enabled = true;
        SwitchState(PlayerState.Idle);
    }
   
    //切换状态
    public void SwitchState(PlayerState state)
    {
        switch (state) 
        {
            case PlayerState.Idle:
                stateMachine.EnterState<PlayerIdleState>();
                break;
            case PlayerState.Move:
                stateMachine.EnterState<PlayerMoveState>();
                break;
            case PlayerState.Hover:
                stateMachine.EnterState<PlayerHoverState>();
                break;
            case PlayerState.Aiming:
                stateMachine.EnterState<PlayerAimingState>();
                break;
          
        }
        currentState = state;
    }
    //播放动画
    public void PlayerStateAnimation(string animationName , float transition = 0.25f, int layer = 0)
    {
        anim.CrossFadeInFixedTime(animationName, transition, layer);
    }
    //判断是否悬空
    public bool IsHover()
    {
        return !Physics.Raycast(transform.position,Vector3.down,fallHeight);//当高度大于fallheight时，处于悬空状态
    }
    //计算模型前三帧的平均速度
    private void UpdateAverageCacheSpeed(Vector3 newSpeed)
    {
        speedCache[speedCache_index++] = newSpeed;
        speedCache_index %= CACHE_SIZE;//防止数组越界
        //计算缓存池中的平均速度
        Vector3 sum = Vector3.zero;
        foreach (Vector3 cache in speedCache)
        {
            sum += cache;
        }
        averageDeltaMovement = sum / CACHE_SIZE;
    }

    private void OnAnimatorMove()
    {
        Vector3 playerDeltaMovement = anim.deltaPosition; //获取动画控制器当前帧位置信息
        if(currentState != PlayerState.Hover)//若不悬空，记录速度信息
        {
            UpdateAverageCacheSpeed(anim.velocity);
        }
        else
        {
            playerDeltaMovement = averageDeltaMovement * Time.deltaTime;//悬空时
        }
            playerDeltaMovement.y = verticalSpeed * Time.deltaTime;
        cc.Move(playerDeltaMovement);
    }

    //进入瞄准
    public void EnterAnim()
    {
        //启动瞄准相关约束
        rightHandAimConstraint.weight = 1;
        bodyAimConstraint.weight = 1;
        rightHandConstraint.weight = 0;
    }
    //退出瞄准
    public void ExitAnim() 
    {
        //关闭瞄准相关约束
        rightHandAimConstraint.weight = 0;
        bodyAimConstraint.weight = 0;
        rightHandConstraint.weight = 1;
    }
    //计算与玩家当前控制角色间距离
    public float DistanceOfCurrentPlayerModel()
    {
        return Vector3.Distance(PlayerController.Instance.currentModel.transform.position,transform.position);
    }

    //玩家受伤
    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;  // 已死亡
        //血量减少
        currentHealth -= damage;        
        _lastDamageTime = Time.time;  // 记录受伤时间
        OnHealthChanged?.Invoke(currentHealth, maxHealth);  // 通知 UI
        if (currentHealth <= 0)
        {
            PlayerController.Instance.Die();
        }
    }

   //玩家回血
    public void Heal(int amount)
    {
        if (currentHealth <= 0) return;  // 已死亡
        //血量增加
        currentHealth += amount;   
        if(currentHealth > maxHealth)
            currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    
}
