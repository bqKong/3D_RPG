using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce = 0;

    [Header("Weapon")]
    public GameObject rockPrefab;

    [Header("手的坐标")]
    public Transform handPos;

    //Animation Event
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            //direction.Normalize();

            attackTarget.transform.GetComponent<NavMeshAgent>().isStopped = true;
            attackTarget.transform.GetComponent <NavMeshAgent>().velocity = direction * kickForce;
            attackTarget.transform.GetComponent<Animator>().SetTrigger("Dizzy");

            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    //Animation Event
    public void ThrowRock()
    {
        if (attackTarget != null)
        { 
            var rock = Instantiate(rockPrefab,handPos.position,Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
        }
    }


}
