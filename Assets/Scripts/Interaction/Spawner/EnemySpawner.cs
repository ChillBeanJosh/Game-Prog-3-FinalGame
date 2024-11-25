using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemy;
    public int xPos;
    public int zPos;
    public int enemyCount = 0;
    public int enemyLimit = 10;
    public float spawnTime = 6;

 

    void Start()
    {
        StartCoroutine(EnemyDrop());
    }

    void Update()
    {
        if (enemyCount == enemyLimit)
        {
            enemyCount = 0;
            spawnTime -= 0.5f;
            StartCoroutine(EnemyDrop());

            if(spawnTime <= 0.5f)
            {
                spawnTime = 1.5f;
            }
        }   
     
    }

    IEnumerator EnemyDrop()
    {
        while (enemyCount < enemyLimit)
        {
            xPos = Random.Range(-30, 45);
            zPos = Random.Range(-60, 20);
            Vector3 position1 = new Vector3(xPos, 1, zPos);

            xPos = Random.Range(-30, 45); 
            zPos = Random.Range(-60, 20);
            Vector3 position2 = new Vector3(xPos, 0.05f, zPos);

            Instantiate(enemy[0], position1, Quaternion.identity); 
            Instantiate(enemy[1], position2, Quaternion.identity); 

            yield return new WaitForSeconds(spawnTime);
            enemyCount += 2; 
        }
    }
}
