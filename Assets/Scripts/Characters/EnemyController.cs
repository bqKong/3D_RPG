using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    GUARD,
    PATROL,
    CHASE,
    DEAD
}


[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;

    //左顾右盼时间
    public float lookAtTime;

    [Header("Patrol State")]
    public float patrolRange;

    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;

    private GameObject attackTarget;


    //记录原来速度
    private float speed;

    private Vector3 wayPoint;

    //初始坐标
    private Vector3 guardPos;

    private float remainLookAtTime;

    //bool 配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        speed = agent.speed;
        guardPos = transform.position;
        remainLookAtTime = lookAtTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }

    }

    // Update is called once per frame
    void Update()
    {
        SwitchStates();
        SwitchAnimation();

    }

    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("isChase", isChase);
        anim.SetBool("Follow", isFollow);

    }

    private void SwitchStates()
    {
        //发现player 切换到CHASE
        if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:

                break;

            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否到了随机巡逻队
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }

                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }

                break;

            case EnemyStates.CHASE:

                isWalk = false;
                isChase = true;
                agent.speed = speed;
                //TODO:追player
                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        //保持原位不动
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        enemyStates = EnemyStates.PATROL;
                    }
                }

                //追击
                else
                {
                    isFollow = true;
                    agent.destination = attackTarget.transform.position;
                }

                //TODO:在攻击范围内攻击，配合动画

                break;

            case EnemyStates.DEAD:

                break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }

        }

        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    { 
    
    }

    bool TargetInSkillRange()
    { 
    
        
    }



    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        //随机生成新的点
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        //FIXME:可能有问题
        //wayPoint = randomPoint;

        //修复问题
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }


}
