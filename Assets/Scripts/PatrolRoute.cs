using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatrolRoute : MonoBehaviour
{

    [SerializeField] bool hasSpecificEnemies;
    [SerializeField] GameObject[] enemies;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemyIdle = collision.GetComponent<EnemyPatrol>();
        if (!enemyIdle) { return; }
        if (!hasSpecificEnemies)
        {
            enemyIdle.Flip();
        }
        else
        {
            if (IsAnEnemyInTheList(collision.gameObject))
            {
                enemyIdle.Flip();
            }
        }
    }
     

    private bool IsAnEnemyInTheList(GameObject targetGameObject)
    {
        foreach(var enemy in enemies)
        {
            if(enemy == targetGameObject) { return true; }
        }
        return false;
    }
}
