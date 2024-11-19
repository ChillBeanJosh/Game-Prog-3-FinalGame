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

    public float rotationSpeed;

    public LayerMask hitLayer;
    public GameObject HitBoxLocation;
    public float HitboxRadius;

    public float currentSpeed;
    public float ArcherTurnRate;

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
        WithinFireRange();

        Vector3 dir = (TargetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ArcherTurnRate * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

    }


    void PrepareFire()
    {
        currentSpeed = 0;
        LookAt(TargetPosition);


    }


    void Fire()
    {

    }

    void Readjust()
    {

    }

    public void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    //visualize range.
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

}