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

    public GameObject breakEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    public void FlyToTarget()
    {
        if (target == null)
            target = FindObjectOfType<PlayerController>().gameObject;

        direction = (transform.transform.position - transform.position + Vector3.up).normalized;
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
                    other.gameObject.GetComponent<NavMeshAgent>().velocity = direction * force;

                    other.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, other.gameObject.GetComponent<CharacterStats>());

                    rockStates = RockState.HitNothing;
                }
                break;

            case RockState.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
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
