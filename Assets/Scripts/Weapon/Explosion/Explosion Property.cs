using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionProperty : MonoBehaviour
{
    [Header("Explosion Properties")]
    public float maxSize = 5f;             
    public float growthSpeed = 2f;         
    public float timeTillDestroy = 2f;     

    [Header("Launch Properties")]
    public float launchForce = 10f;  
    public float enemylaunchForce = 4f;       
    public float launchDuration = 1f;      
    public float gravity = -9.8f;          

    private bool isGrowing = true;   

    public float ragdollDelay;   
    public float ragdollDestroy;

    private void Update()
    {
       
        if (isGrowing)
        {
            transform.localScale += Vector3.one * growthSpeed * Time.deltaTime;

            if (transform.localScale.x >= maxSize)
            {
                transform.localScale = new Vector3(maxSize, maxSize, maxSize);
                isGrowing = false;

                Invoke(nameof(DestroyObject), timeTillDestroy);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player check.
        Player player = other.GetComponent<Player>();
        CharacterController characterController = other.GetComponent<CharacterController>();
        if (characterController != null)
        {
            player.TakeDamage(10);
            StartCoroutine(LaunchCharacterController(characterController, other.transform));
            return;
        }

        // Enemy check.
        if (other.gameObject.layer == LayerMask.NameToLayer("enemy"))
        {
            Rigidbody enemyRb = other.gameObject.GetComponent<Rigidbody>();
            enemyRagdoll ragdoll = other.gameObject.GetComponent<enemyRagdoll>();

            if (ragdoll != null)
            {
                // If ragdoll is found, proceed with the launch and ragdoll logic
                StartCoroutine(RagdollDelay(ragdoll, ragdollDelay));

                Vector3 launchDirection = (other.transform.position - transform.position).normalized;
                StartCoroutine(LaunchObject(enemyRb, launchDirection));

                // Destroys the enemy after the specified time frame.
                Destroy(other.gameObject, ragdollDestroy);

                if (ScoreCollector.Instance != null)
                {
                    ScoreCollector.Instance.AddScore(1); // Increment score by 1
                }
            }
            else
            {
                Debug.LogWarning("No enemyRagdoll found on " + other.gameObject.name);
            }
        }
    }

    private System.Collections.IEnumerator LaunchObject(Rigidbody rb, Vector3 direction)
    {
        float elapsedTime = 0f;                
        Vector3 initialPosition = rb != null ? rb.position : Vector3.zero; // Ensure initialPosition has a fallback if rb is null
        Vector3 velocity = direction * enemylaunchForce; 

        while (elapsedTime < launchDuration)
        {
            if (rb == null) // Null check to ensure the Rigidbody still exists
            {
                yield break; // Exit the coroutine early if the Rigidbody is destroyed
            }

            Vector3 displacement = velocity * elapsedTime + 0.5f * new Vector3(0, gravity, 0) * Mathf.Pow(elapsedTime, 2);
            rb.MovePosition(initialPosition + displacement);

            elapsedTime += Time.deltaTime; 
            yield return null;
        }
    }

    private System.Collections.IEnumerator LaunchCharacterController(CharacterController controller, Transform transform)
    {
        float elapsedTime = 0f;               
        Vector3 initialPosition = transform.position; 
        Vector3 velocity = (transform.position - this.transform.position).normalized * launchForce; 

        while (elapsedTime < launchDuration)
        {
            Vector3 displacement = velocity * Time.deltaTime + new Vector3(0, gravity, 0) * Mathf.Pow(elapsedTime, 2) * Time.deltaTime;

            controller.Move(displacement);

            elapsedTime += Time.deltaTime; 
            yield return null;
        }
    }

    private IEnumerator RagdollDelay(enemyRagdoll ragdoll, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (ragdoll != null)
        {
            ragdoll.KnockbackActive();
        }
        else
        {
            Debug.LogWarning("enemyRagdoll component is missing from the enemy.");
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}

