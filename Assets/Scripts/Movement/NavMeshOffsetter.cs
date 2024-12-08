using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshOffsetter : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent navMeshAgent;
    float navMeshOffset;

    void Awake()
    {
        navMeshAgent = this.gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        navMeshAgent.baseOffset = navMeshOffset; //Not sure if this is correct off the top of my head
    }
}
