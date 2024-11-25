using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherProjectile : MonoBehaviour
{
    public float arrowSpeed;
    public float despawnTime;

    private Vector3 initialDirection;

    public void Initialize(Vector3 direction)
    {
        initialDirection = direction.normalized;
    }

    void Update()
    {
        transform.position += initialDirection * arrowSpeed * Time.deltaTime;
        Destroy(gameObject, despawnTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collision object is the player
        if (other.CompareTag("Player"))
        {
            // Get the Player script and apply damage
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(10);  // Apply damage, adjust the value as needed
            }

            // Destroy the arrow after hitting the player
            Destroy(gameObject);
        }
    }
}
