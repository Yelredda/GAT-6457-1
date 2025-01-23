using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NetController : MonoBehaviour {

    private EnemyMovement enemyMovement;

    void OnTriggerEnter(Collider other) { 
        if (other.CompareTag("Enemy")) {
            enemyMovement = other.transform.parent.GetComponent<EnemyMovement>();
            Debug.Log("Freeze Enemy!");
            enemyMovement.Freeze();
        }
    }

    private void OnDestroy() {
        if (enemyMovement != null) {
            enemyMovement.UnFreeze();
        }
    }
}
