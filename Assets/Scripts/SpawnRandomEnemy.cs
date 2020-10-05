using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomEnemy : MonoBehaviour
{
    public GameObject[] enemyList;
    void Awake()
    {
        GameObject enemy = Instantiate(enemyList[Random.Range(0, enemyList.Length)], transform.position,
            Quaternion.identity);
    }
}
