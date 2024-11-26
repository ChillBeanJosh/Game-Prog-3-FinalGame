using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerToggle : MonoBehaviour
{
    private EnemySpawner spawner;

    private void Start()
    {
        spawner = FindAnyObjectByType<EnemySpawner>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            spawner.enabled = true;
        }
        else
        {
            spawner.enabled = false;
        }

    }
}
