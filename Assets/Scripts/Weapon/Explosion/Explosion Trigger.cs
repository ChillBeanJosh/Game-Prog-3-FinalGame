using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTrigger : MonoBehaviour
{
    public GameObject explosionObject;
    public LayerMask targetLayer;

    public Vector3 spawnOffset = Vector3.zero;

    public float destructionDelay = 5f;  

    private void Start()
    {
        StartCoroutine(ExplosionTimer());
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            Instantiate(explosionObject, transform.position + spawnOffset, Quaternion.identity);

            weaponCollector collector = other.GetComponent<weaponCollector>();
            if (collector != null)
            {
                collector.DecrementWeaponCount(); 
                Destroy(gameObject);
            }
        }
    }

    private IEnumerator ExplosionTimer()
    {
        yield return new WaitForSeconds(destructionDelay);

        weaponCollector collector = GetComponent<weaponCollector>();
        if (collector != null)
        {
            collector.DecrementWeaponCount(); 
            Destroy(gameObject);
        }

    }
}
