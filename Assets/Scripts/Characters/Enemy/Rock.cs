using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Rock : MonoBehaviour
{

    public enum RockState
    {
        HitPlayer,
        HitEnemy,
        HitNothing
    }

    private Rigidbody rb;
    public RockState rockStates;

    [Header("Basic Settings")]
    public float force;
    public GameObject target;
    public Vector3 direction;
    public int damage;

    [Tooltip("粒子特效")]
    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //防止判断速度小于1f进入HitNothing的状态
        rb.velocity = Vector3.one;

        rockStates = RockState.HitPlayer;
        FlyToTarget();
    }


    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1f)
        {
            rockStates = RockState.HitNothing;
        }
    }

    /// <summary>
    /// 石头飞向攻击目标
    /// </summary>
    public void FlyToTarget()
    {
        //获取target的gameobject
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;

        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);

    }


    private void OnCollisionEnter(Collision other)
    {

        switch (rockStates)
        {
            case RockState.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    other.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    //击退player
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");

                    //受伤
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockState.HitNothing;
                }
                break;

            case RockState.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    //获取石头人身上的CharacterStats组件
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();

                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    //销毁石头
                    Destroy(gameObject);
                }
                break;

            case RockState.HitNothing:

                break;

        }

    }

}
