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
        Recovering
    }

    public ChargerState currentState;

    public float rotationSpeed = 5f;
    public float currentSpeed = 0f;
    public float holdMaxSpeedCurrentTime = 0f;

    public bool atMaxSpeed = false;
    public bool hasStopped = false;

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

        currentState = ChargerState.PreparingCharge;
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
        }
    }

    void PreparingCharge()
    {
        LookAt(TargetPosition);

        float approxAngle = Vector3.Angle(transform.forward, (TargetPosition - transform.position).normalized);

        if (approxAngle < 15f)
        {
            currentState = ChargerState.Charging;
        }
    }

    void Charging()
    {
        CheckForHits();

        Vector3 dir = (TargetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ChargeTurnRate * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

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

        Vector3 dir = (TargetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, SlowDownTurnRate * Time.deltaTime);
        transform.position += transform.forward * currentSpeed * Time.deltaTime;

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

    void HandleRecovery()
    {
        recoveryTimer += Time.deltaTime;

        if (recoveryTimer >= recoveryTime)
        {
            ResetForNextCharge();
        }
    }

    void ResetForNextCharge()
    {
        currentSpeed = 0f;
        holdMaxSpeedCurrentTime = 0f;
        atMaxSpeed = false;
        hasStopped = false;
        recoveryTimer = 0f;
        currentState = ChargerState.PreparingCharge;
    }

    public void LookAt(Vector3 target)
    {
        Vector3 dir = target - transform.position;

        float angle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, angle, 0);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
                Debug.Log("Player hit detected!");

                Vector3 knockbackDirection = (hitPlayer.transform.position - transform.position).normalized;
                ApplyKnockback(hitPlayer.gameObject, knockbackDirection, KnockbackForce);

                hitPlayer.TakeDamage(15);

                currentState = ChargerState.Recovering;
                recoveryTimer = 0f;
            }
        }
    }

    private void ApplyKnockback(GameObject target, Vector3 direction, float force)
    {
        CharacterController controller = target.GetComponent<CharacterController>();

        if (controller != null)
        {
            Vector3 knockbackVector = direction * force * Time.deltaTime;

            RaycastHit hit;
            bool isBlocked = Physics.Raycast(target.transform.position, direction, out hit, knockbackVector.magnitude + 0.1f);

            if (isBlocked)
            {
                controller.Move(direction * (hit.distance - 0.1f));
            }
            else
            {
                controller.Move(knockbackVector * 20);
            }
        }
    }
}




