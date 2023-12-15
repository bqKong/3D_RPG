using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    // Start is called before the first frame update
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    /// <summary>
    /// 移动方法
    /// </summary>
    /// <param name="target"></param>
    public void MoveToTarget(Vector3 target)
    { 
        agent.destination = target;
    }

}
