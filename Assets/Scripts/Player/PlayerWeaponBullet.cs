using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponBullet : MonoBehaviour
{
    [HideInInspector]
    public Rigidbody rb;
    [Tooltip("伤害")]
    public int damage = 10;
    [Tooltip("推力")]
    public float flyPower = 100f;
    [Tooltip("存活时间")]
    public float lifeTime = 10f;

    private Vector3 prevPosition;

    [Tooltip("子弹气流预制体")]
    public GameObject trailEffect;
    [Tooltip("气流生成间隔")]
    public float trailInterval = 0.1f;
    public float trailInterval_timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rb.velocity = transform.forward * flyPower;
        Destroy(gameObject,lifeTime);
        CheckInitiaOverLap();
    }
    private void Update()
    {
        CheckCollision();
        prevPosition = transform.position;
        trailInterval_timer += Time.deltaTime;
        if(trailInterval_timer >= trailInterval)
        {
            SpawnTrailEffect();
            trailInterval_timer = 0;
        }
    }

    private void SpawnTrailEffect()
    {
        if (trailEffect != null)
        {
            Quaternion reverseRotation = Quaternion.LookRotation(-transform.forward);
            Destroy(Instantiate(trailEffect,transform.position,reverseRotation),1f);

        }
    }
    //检查子弹是否生成在敌人内部
    private void CheckInitiaOverLap()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 0.1f);
        foreach(var hitCollider in hitColliders)
        {
            EnemyModel enemy = hitCollider.GetComponent<EnemyModel>();
            if(enemy != null)
            {
                enemy.Hurt(this, 1);
                Destroy(gameObject);
                return; 
            }
        }
    }

    private void CheckCollision()
    {
        RaycastHit hit;
        Vector3 dir = transform.position - prevPosition;//子弹方向
        float distance = Vector3.Distance(transform.position,prevPosition);//两帧之间子弹飞行距离
        if (Physics.Raycast(prevPosition,dir.normalized,out hit ,distance))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                EnemyModel enemy = hit.collider.GetComponent<EnemyModel>();
                enemy.Hurt(this,1);
            }
        }
    }
}
