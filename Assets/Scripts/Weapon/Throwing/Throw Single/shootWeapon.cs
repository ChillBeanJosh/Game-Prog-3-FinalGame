using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootWeapon : MonoBehaviour
{
    public KeyCode throwKey = KeyCode.Mouse0;

    public float throwCooldown = 1.0f;
    private float nextThrowTime = 0f;

    private weaponCollector collector;
    public Camera playerCamera;

    void Start()
    {
        collector = GetComponent<weaponCollector>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(throwKey) && Time.time >= nextThrowTime)
        {
            ThrowPrefab();
        }
    }

    void ThrowPrefab()
    {
        if (collector.GetWeaponCount() > 0)
        {
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                if (child != transform && child.CompareTag("Weapon"))
                {
                    Debug.Log("Weapon Thrown");
                    StartCoroutine(collector.ThrowWeapon(child.gameObject));
                    nextThrowTime = Time.time + throwCooldown;
                    //break so it is one at a time.
                    break;
                }
            }
        }
    }
}
