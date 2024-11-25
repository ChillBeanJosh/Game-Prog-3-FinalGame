using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponSpawner : MonoBehaviour
{

    public KeyCode spawnWeapon = KeyCode.Q;
    public GameObject[] weapons;

    //for determining if the weapons are childs to the player.
    public bool childToggle = false;

    public float weaponSpacing = 2.0f;

    public float cooldown = 7.0f;
    private float cdTime;

    private weaponCollector collector;

    void Start()
    {
        collector = FindObjectOfType<weaponCollector>();
    }

    void Update()
    {
        if (Input.GetKeyDown(spawnWeapon) && Time.time >= cdTime)
        {
            SpawnWeapons();
            Debug.Log("WEAPONS SPAWNED!");
            cdTime = Time.time + cooldown;
        }
    }

    void SpawnWeapons()
    {
        if (weapons.Length < 6)
        {
            Debug.LogError("Assign at least 6 weapons in the weapons array.");
            return;
        }

        // Spawning weapons on the left
        for (int i = 0; i < 3; i++)
        {
            Vector3 spawnPosition = transform.position + transform.right * -(i + 1) * weaponSpacing;
            GameObject newWeapon = Instantiate(weapons[i], spawnPosition, Quaternion.identity);

            SetupWeapon(newWeapon);
        }

        // Spawning weapons on the right
        for (int i = 3; i < 6; i++)
        {
            Vector3 spawnPosition = transform.position + transform.right * (i - 2) * weaponSpacing;
            GameObject newWeapon = Instantiate(weapons[i], spawnPosition, Quaternion.identity);

            SetupWeapon(newWeapon);
        }
    }

    void SetupWeapon(GameObject weapon)
    {
        if (childToggle)
        {
            weapon.transform.SetParent(transform);

            // Automatically collect weapons if collector exists
            if (collector != null)
            {
                collector.CollectWeapon(weapon);
            }
        }
        else
        {
            // Ensure weapon has the necessary components for interaction
            Rigidbody rb = weapon.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = weapon.AddComponent<Rigidbody>();
                rb.isKinematic = true; // Prevent physics interactions initially
            }

            Collider col = weapon.GetComponent<Collider>();
            if (col == null)
            {
                weapon.AddComponent<BoxCollider>(); // Add a basic collider if none exists
            }
        }
    }
}

