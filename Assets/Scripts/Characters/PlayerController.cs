using System;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
    }


    // Update is called once per frame
    void Update()
    {
        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3 target)
    { 
        StopAllCoroutines();
        agent.isStopped = false;
        agent.destination = target;
    }

    public void EventAttack(GameObject target)
    {
        if (target != null)
        {
            attackTarget = target;

            //TODO:移动到面前并攻击
            //不停移动直到站到怪物跟前，不用能while，要用协程
            StartCoroutine(MoveToAttackTarget());

        }

    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);

        //TODO:修改范围参数
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
            anim.SetTrigger("Attack");

            //重置冷却时间
            lastAttackTime = 0.5f;
        }

    }

}
