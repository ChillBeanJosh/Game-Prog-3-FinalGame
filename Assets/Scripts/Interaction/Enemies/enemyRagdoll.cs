using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyRagdoll : MonoBehaviour
{
    public Animator enemyAnimator;
    public CapsuleCollider enemyBoxCollider;
    public Rigidbody enemyRigidbody;

    private Rigidbody[] ragdollRigidbody;
    private Collider[] ragdollCollider;

    private bool isRagdollActive = false; // New flag to track if ragdoll is active

    private void Start()
    {
        ragdollRigidbody = GetComponentsInChildren<Rigidbody>();
        ragdollCollider = GetComponentsInChildren<Collider>();

        DisabledRagdoll();
    }

    public void KnockbackActive()
    {
        if (isRagdollActive) return; // If ragdoll is already active, do nothing

        enemyAnimator.enabled = false;
        enemyBoxCollider.enabled = false;
        enemyRigidbody.isKinematic = true;

        ActivatedRagdoll();
    }

    private void ActivatedRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbody)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in ragdollCollider)
        {
            if (col != enemyBoxCollider)
            {
                col.enabled = true;
            }
        }

        isRagdollActive = true; // Mark ragdoll as active
    }

    private void DisabledRagdoll()
    {
        foreach (Rigidbody rb in ragdollRigidbody)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in ragdollCollider)
        {
            if (col != enemyBoxCollider)
            {
                col.enabled = false;
            }
        }

        isRagdollActive = false; // Mark ragdoll as inactive
    }
}
