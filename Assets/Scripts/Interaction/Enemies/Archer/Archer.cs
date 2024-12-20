using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Charger;

public class Archer : MonoBehaviour
{
   public enum ArcherState
    {
        MovingToTarget,
        PrepareFire,
        Shooting,
        Readjust
    }

    public ArcherState currentState;

    private Vector3 TargetPosition;
    private Rigidbody rb;
    private GameObject player;

    public GameObject arrowObject;
    public Transform arrowSpawnPosition;

    public Animator animator;

    public float rotationSpeed;

    public LayerMask hitLayer;
    public GameObject HitBoxLocation;
    public float HitboxRadius;

    public float currentSpeed;
    public float ArcherTurnRate;

    public float BowDrawTime;

    public bool hasFired = false;

    public float runAwaySpeed;

    public LayerMask groundLayer; // Layer mask to detect ground
    public float groundCheckDistance = 0.1f; // Distance for ground checking

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            TargetPosition = player.transform.position;
        }
        else
        {
            Debug.LogError("Player not found");
        }

        currentState = ArcherState.MovingToTarget;
    }

    private void Update()
    {
        if (player != null) TargetPosition = player.transform.position;

        switch (currentState)
        {
            case ArcherState.MovingToTarget:
                MoveTowardsPlayer();
                break;

            case ArcherState.PrepareFire:
                PrepareFire();
                break;

            case ArcherState.Shooting:
                Fire();
                break;

            case ArcherState.Readjust:
                Readjust();
                break;
        }
    }

    void MoveTowardsPlayer()
    {
        animator.SetBool("RunAway", false);
        animator.SetBool("MoveTo", true);

        currentSpeed = 3.5f;

        WithinFireRange();

        Vector3 dir = (TargetPosition - transform.position).normalized;

        //stays flat.
        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ArcherTurnRate * Time.deltaTime);

        Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
        transform.position += move;

        StayGrounded();
    }

    void PrepareFire()
    {
        if (!hasFired)
        {
            animator.SetBool("RunAway", false);
            animator.SetBool("MoveTo", false);

            animator.SetTrigger("Draw");

            currentSpeed = 0;
            LookAt(TargetPosition);
        }
    }

    void Fire()
    {
        if (!hasFired)
        {
            Debug.Log("Arrow has been fired!");

            Vector3 directionToPlayer = (TargetPosition - arrowSpawnPosition.position).normalized;
            Quaternion arrowRotation = Quaternion.Euler(90, 0, 0);

            float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

            Vector3 adjustedRotation = arrowRotation.eulerAngles;
            adjustedRotation.z = angle;
            arrowRotation = Quaternion.Euler(adjustedRotation);

            GameObject arrow = Instantiate(arrowObject, arrowSpawnPosition.position, arrowRotation);

            arrow.GetComponent<ArcherProjectile>().Initialize(directionToPlayer);

            hasFired = true;
            animator.SetTrigger("Drop Bow");
        }
    }

    void Readjust()
    {
        Debug.Log("Archer is Running Away!");
        StartCoroutine(RunAwayForSeconds(1.5f));
    }

    public void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        dir.y = 0;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void StayGrounded()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundLayer))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y;
            transform.position = pos;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(HitBoxLocation.transform.position, HitboxRadius);
    }

    public void WithinFireRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(HitBoxLocation.transform.position, HitboxRadius, hitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();

            if (hitPlayer != null)
            {
                currentState = ArcherState.PrepareFire;
            }
        }
    }

    IEnumerator RunAwayForSeconds(float duration)
    {
        animator.SetBool("RunAway", true);
        animator.SetBool("MoveTo", false);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            Vector3 dir = (transform.position - TargetPosition).normalized;

            dir.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ArcherTurnRate * Time.deltaTime);

            float adjustedSpeed = runAwaySpeed * Time.deltaTime;

            transform.position += dir * adjustedSpeed;

            StayGrounded();

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        animator.SetBool("RunAway", false);
        animator.SetBool("MoveTo", true);
        currentState = ArcherState.MovingToTarget;
    }

    IEnumerator EnterFireState()
    {
        Debug.Log("Preparing Draw...");
        yield return new WaitForSeconds(BowDrawTime);

        Debug.Log("Transitioning to Fire State.");
        animator.ResetTrigger("Draw");

        currentState = ArcherState.Shooting;
    }

    // Controlled through animation events.
    public void EnterReadjust()
    {
        Debug.Log("Transitioning to Readjust State.");
        animator.ResetTrigger("Drop Bow");

        hasFired = false;
        currentState = ArcherState.Readjust;
    }

}