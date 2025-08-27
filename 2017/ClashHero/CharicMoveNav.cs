using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CharicMoveNav : MonoBehaviour {

    public Transform target;
    NavMeshAgent agent;


	// Use this for initialization
	void Start () {

        agent = GetComponent<NavMeshAgent>();
        //agent.speed = 
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.anyKey)
        {
            agent.SetDestination(target.position);
            
        }

        //agent.SetDestination(target.position);
    }
}
