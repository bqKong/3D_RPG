using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce = 30;

    /// <summary>
    /// 击飞
    /// </summary>
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);

            //击飞方向
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();

            attackTarget.GetComponent<NavMeshAgent>().isStopped = true;

            //击飞
            attackTarget.GetComponent<NavMeshAgent>().velocity = direction * kickForce;

            //player眩晕
            attackTarget.GetComponent<Animator>().SetTrigger("Dizzy");

        }

    }

}
