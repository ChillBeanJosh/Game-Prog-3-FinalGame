using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponInteraction : MonoBehaviour
{
    [SerializeField] private weaponSpawner spawner;
    [SerializeField] private weaponCollector collector;

    private void Start()
    {
        spawner = FindObjectOfType<weaponSpawner>();
        collector = FindObjectOfType<weaponCollector>();
    }

    //used for weapon picked.
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!spawner.childToggle)
            {
                collector.CollectWeapon(gameObject);
            }
            else
            {
                collector.CollectWeapon(gameObject);
            }
        }
    }
}
