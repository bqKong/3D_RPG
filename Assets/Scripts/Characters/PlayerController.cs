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

    private bool isDead;

    private float stopDistance;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();

        stopDistance = agent.stoppingDistance;
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;

        //注册GameManager
        GameManager.Instance.RigisterPlayer(characterStats);
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

        if (isDead) return;

       // agent.stoppingDistance = stopDistance;

        agent.isStopped = false;
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (isDead) return;

        if (target != null)
        {
            attackTarget = target;

            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;

            //移动到面前并攻击
            //不停移动直到站到怪物跟前，不用能while，要用协程
            StartCoroutine(MoveToAttackTarget());

        }

    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;

        //agent.stoppingDistance = characterStats.attackData.attackRange;

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
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats, targetStats);
    }


}
