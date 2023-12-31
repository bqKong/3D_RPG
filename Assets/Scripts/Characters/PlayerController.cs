using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{

    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;

    private GameObject attackTarget;

    [Tooltip("冷却时间")]
    private float lastAttackTime;
    [Tooltip("人物是否死亡")]
    private bool isDead;
    [Tooltip("停止距离")]
    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    private void OnEnable()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        //将人物注册到GameManager
        GameManager.Instance.RigisterPlayer(characterStats);
    }

    // Start is called before the first frame update
    void Start()
    {
        //一开始，player拿到自己的数据
        SaveManager.Instance.LoadPlayerData();
    }


    //人物销毁要注销掉之前的事件，否则会一直存在于系统当中
    private void OnDisable()
    {
        MouseManager.Instance.OnMouseClicked -= MoveToTarget;
        MouseManager.Instance.OnEnemyClicked -= EventAttack;
    }


    // Update is called once per frame
    void Update()
    {
        isDead = characterStats.CurrentHealth == 0;

        //人物死亡，广播，通知所有挂载这个接口代码的函数
        //让他们知道player结束了，它们要执行游戏结束的方法
        if (isDead)
        {
            GameManager.Instance.NotifyObservers();
        }

        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();

        //如果死亡就返回
        if (isDead) return;
        //恢复初始的停止距离
        agent.stoppingDistance = stopDistance;

        agent.isStopped = false;
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;

            //是否暴击了
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            //不停移动直到站到怪物跟前，不用能while，要用协程
            StartCoroutine(MoveToAttackTarget());

        }

    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        //攻击时，将当前的停止距离修改为攻击距离
        agent.stoppingDistance = characterStats.attackData.attackRange;
        //保证攻击时player是面朝敌人的
        transform.LookAt(attackTarget.transform);

        while (Vector3.Distance(attackTarget.transform.position, this.transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        //停下来
        agent.isStopped = true;

        //Attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            //重置冷却时间
            lastAttackTime = characterStats.attackData.coolDown;
        }

    }

    //Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockState.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockState.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
               attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20f, ForceMode.Impulse);
            }

        }
        else
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }

    }

}
