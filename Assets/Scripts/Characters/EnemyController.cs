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
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;

    [Tooltip("原地左顾右盼时间")]
    public float lookAtTime;

    [Header("Patrol State")]
    public float patrolRange;

    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    protected GameObject attackTarget;

    protected CharacterStats characterStats;


    //记录原来速度
    private float speed;

    //目的坐标
    private Vector3 wayPoint;

    //初始坐标
    private Vector3 guardPos;

    private float remainLookAtTime;

    //攻击冷却时间
    private float lastAttackTime;

    //初始的旋转角度(四元数)
    private Quaternion guardRotation;

    //bool 配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;

    [Tooltip("player是否死亡")]
    bool playerDead;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        //获取初始的坐标
        guardPos = transform.position;
        //获取初始角度
        guardRotation = transform.rotation;
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

        //FIXME:场景切换后会修改
        GameManager.Instance.AddObserver(this);

    }

    //敌人生成加入列表
    //private void OnEnable()
    //{
    //    GameManager.Instance.AddObserver(this);
    //}

    //敌人死亡移除列表
    private void OnDisable()
    {
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (characterStats.CurrentHealth == 0)
        {
            isDead = true;
        }

        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }


    }

    /// <summary>
    /// 动画函数
    /// </summary>
    private void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    /// <summary>
    /// 模式切换函数
    /// </summary>
    private void SwitchStates()
    {
        //如果死亡切换状态
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }

        //发现player 切换到CHASE
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch (enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.Distance(guardPos, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        //防止瞬间转身不自然
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.1f);
                    }

                }
                break;

            case EnemyStates.PATROL:

                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否到了随机巡逻点
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
                //追player
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
                    //跑着追击player
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                //在攻击范围内攻击，配合动画
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }

                }
                break;

            case EnemyStates.DEAD:
                coll.enabled = false;
                //agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }


    /// <summary>
    /// 执行攻击
    /// </summary>
    void Attack()
    {
        //攻击要看着目标
        transform.LookAt(attackTarget.transform);

        if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }

        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }


    }


    /// <summary>
    /// 发现玩家
    /// </summary>
    /// <returns></returns>
    bool FoundPlayer()
    {
        //找到半径内的所有碰撞体
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

    /// <summary>
    /// 目标是否在攻击范围内
    /// </summary>
    /// <returns></returns>
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.attackRange;
        else return false;
    }

    /// <summary>
    /// 目标是否在技能范围内
    /// </summary>
    /// <returns></returns>
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStats.attackData.skillRange;
        else return false;

    }

    /// <summary>
    /// 重新获取下个目标点
    /// </summary>
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

    //Animation Event
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }

    }

    public void EndNotify()
    {
        //player已死
        playerDead = true;

        //获胜动画
        anim.SetBool("Win", true);

        //停止所有移动
        //停止Agent
        isChase = false;
        isWalk = false;
        attackTarget = null;

    }
}
