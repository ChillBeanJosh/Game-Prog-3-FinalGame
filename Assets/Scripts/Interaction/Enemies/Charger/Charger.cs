using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : MonoBehaviour
{
    public enum ChargerState
    {
        PreparingCharge,
        Charging,
        SlowingDown,
        Recovering,
        Impact
    }

    public ChargerState currentState;

    public float rotationSpeed = 5f;
    private float currentSpeed = 0f;
    private float holdMaxSpeedCurrentTime = 0f;

    private bool atMaxSpeed = false;
    private bool hasStopped = false;

    private float acceleration;
    private float slowAcceleration;

    public float ChargeSpeed;
    public float SpeedHoldTime;

    public float ChargeDuration;
    public float ChargeTurnRate;

    public float SlowDownDuration;
    public float SlowDownTurnRate;

    public LayerMask hitLayer;
    public GameObject HitBoxLocation;
    public float HitboxRadius;

    public float KnockbackForce;

    private Vector3 TargetPosition;
    private Rigidbody rb;
    private GameObject player;

    public float recoveryTime;
    private float recoveryTimer = 0f;

    public Animator animator;

    public LayerMask groundLayer; 
    public float groundCheckDistance = 0.1f;

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

        acceleration = ChargeSpeed / ChargeDuration;
        slowAcceleration = ChargeSpeed / SlowDownDuration;

        currentState = ChargerState.Recovering;
    }

    private void Update()
    {
        if (player != null) TargetPosition = player.transform.position;

        switch (currentState)
        {
            case ChargerState.PreparingCharge:
                PreparingCharge();
                break;

            case ChargerState.Charging:
                Charging();
                break;

            case ChargerState.SlowingDown:
                SlowdownCharge();
                break;

            case ChargerState.Recovering:
                HandleRecovery();
                break;

            case ChargerState.Impact:
                break;
        }
    }

    void PreparingCharge()
    {
        animator.SetTrigger("Point");

        LookAt(TargetPosition);
    }

    public void OnPointAnimationEnd()
    {
        float approxAngle = Vector3.Angle(transform.forward, (TargetPosition - transform.position).normalized);

        if (approxAngle < 15f)
        {
            currentState = ChargerState.Charging;
        }
    }

    void Charging()
    {
        CheckForHits();

        animator.SetBool("isCharging", true);
        animator.speed = currentSpeed / ChargeSpeed;

        Vector3 dir = (TargetPosition - transform.position).normalized;

        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ChargeTurnRate * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        StayGrounded();

        if (!atMaxSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, ChargeSpeed, acceleration * Time.deltaTime);

            if (currentSpeed >= ChargeSpeed)
            {
                atMaxSpeed = true;
            }
        }
        else
        {
            holdMaxSpeedCurrentTime += Time.deltaTime;

            if (holdMaxSpeedCurrentTime >= SpeedHoldTime)
            {
                currentState = ChargerState.SlowingDown;
            }
        }
    }

    void SlowdownCharge()
    {
        CheckForHits();

        animator.SetBool("isCharging", true);

        Vector3 dir = (TargetPosition - transform.position).normalized;

        dir.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, SlowDownTurnRate * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

        StayGrounded();

        if (!hasStopped)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, slowAcceleration * Time.deltaTime);

            if (currentSpeed <= 0f)
            {
                hasStopped = true;
            }
        }
        else
        {
            currentState = ChargerState.Recovering;
        }
    }

    void EnterRecovery()
    {
        currentState = ChargerState.Recovering;
        animator.SetBool("isIdle", true);
    }

    void HandleRecovery()
    {
        recoveryTimer += Time.deltaTime; 

        animator.SetBool("isCharging", false); 
        animator.SetBool("isIdle", true); 

        if (recoveryTimer >= recoveryTime) 
        {
            ResetForNextCharge();
            currentState = ChargerState.PreparingCharge;
        }
    }

    void ResetForNextCharge()
    {
        currentSpeed = 0f;
        holdMaxSpeedCurrentTime = 0f;
        atMaxSpeed = false;
        hasStopped = false;
        recoveryTimer = 0f; 
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

    public void CheckForHits()
    {
        Collider[] hitColliders = Physics.OverlapSphere(HitBoxLocation.transform.position, HitboxRadius, hitLayer);

        foreach (var hitCollider in hitColliders)
        {
            Player hitPlayer = hitCollider.GetComponent<Player>();

            if (hitPlayer != null)
            {
                animator.SetTrigger("Smash");

                animator.SetBool("isCharging", false);
                animator.SetBool("isIdle", false);
                Debug.Log("Player hit detected!");

                Vector3 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
                ApplyKnockback(hitPlayer.gameObject, knockbackDirection, KnockbackForce);

                hitPlayer.TakeDamage(15);

                currentState = ChargerState.Impact;
            }
        }
    }

    private void ApplyKnockback(GameObject target, Vector3 direction, float force)
    {
        CharacterController controller = target.GetComponent<CharacterController>();

        if (controller != null)
        {
            Vector3 knockbackVector = (direction.normalized + Vector3.up * 0.0f).normalized * force * Time.deltaTime;

            RaycastHit hit;
            bool isBlocked = Physics.Raycast(target.transform.position, direction, out hit, knockbackVector.magnitude + 0.1f);

            if (isBlocked)
            {
                controller.Move((direction.normalized + Vector3.up * 0.5f).normalized * (hit.distance - 0.1f));
            }
            else
            {
                controller.Move(knockbackVector * 20);
            }
        }
    }
}




