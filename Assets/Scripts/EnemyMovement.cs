using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour {
    public Transform player;
    private NavMeshAgent navMeshAgent;

    // Added to facilitate the net stopping/starting the enemy
    // Additional check during Update()
    private bool isNotFrozen = true;

    // Start is called before the first frame update
    void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update() {
        if (player != null && isNotFrozen) {
            navMeshAgent.SetDestination(player.position);
        }
    }

    // Added to facilitate the net stopping/starting the enemy
    public void Freeze() {
        navMeshAgent.isStopped = true;
        isNotFrozen = false;
    }

    public void UnFreeze() {
        navMeshAgent.isStopped = false;
        isNotFrozen = true;
    }
}
