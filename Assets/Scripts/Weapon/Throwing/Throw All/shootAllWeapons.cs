using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootAllWeapons : MonoBehaviour
{
    public KeyCode shootAll = KeyCode.F;

    [Header("Cooldown Settings")]
    public float shootAllCooldown = 5f;
    private float nextShootTime = 0f;

    [Header("Shooting Settings")]
    public float throwForce = 20f;
    private weaponCollector collector;

    public Camera playerCamera;
    public Transform reticleTransform;

    public Animator animator;


    private void Start()
    {
        collector = GetComponent<weaponCollector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(shootAll) && Time.time >= nextShootTime)
        {
            StartCoroutine(ShootAll());

            Debug.Log("SHOOTING ALL WEAPONS!");

            nextShootTime = Time.time + shootAllCooldown;
        }
    }

    public float rotationSpeed;
    public float destructionDelay = 5f;

    public System.Collections.IEnumerator ShootAll()
    {
        GameObject[] allWeapons = GameObject.FindGameObjectsWithTag("Weapon");

        List<System.Collections.IEnumerator> positionAndRotationCoroutines = new List<System.Collections.IEnumerator>();

        foreach (GameObject weapon in allWeapons)
        {
            if (weapon == null)
            {
                continue;
            }

            if (!weapon.transform.IsChildOf(transform) && weapon != null)
            {
                Rigidbody rb = weapon.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    Quaternion initialRotation = weapon.transform.rotation;

                    Vector3 initialPosition = weapon.transform.position;
                    Vector3 targetPosition = initialPosition + new Vector3(0, 1, 0); //raises hight by y = +1.

                    positionAndRotationCoroutines.Add(UpdateWeaponPositionAndRotation(weapon.transform, initialRotation, targetPosition, rb));
                }
            }
        }

        foreach (var rotationCoroutine in positionAndRotationCoroutines)
        {
            StartCoroutine(rotationCoroutine);
        }

        //delay upon activation/key press.
        animator.SetTrigger("Fire");
        yield return new WaitForSeconds(2.0f);

        foreach (GameObject weapon in allWeapons)
        {
            if (weapon == null)
            {
                continue;
            }

            if (!weapon.transform.IsChildOf(transform))
            {
                Rigidbody rb = weapon.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 shootDirection;

                    if (reticleTransform != null)
                    {
                        shootDirection = (reticleTransform.position - weapon.transform.position).normalized;
                    }
                    else
                    {
                        shootDirection = playerCamera.transform.forward;
                    }

                    weapon.transform.rotation = Quaternion.LookRotation(shootDirection) * Quaternion.AngleAxis(90, Vector3.right);

                    rb.AddForce(shootDirection * throwForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

                    rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;

                    //delay between each projectile shot.
                    yield return new WaitForSeconds(0.001f);

                    StartCoroutine(DestroyWeaponAfterDelay(weapon, destructionDelay));
                }
            }
        }

        yield return null;
    }

    public System.Collections.IEnumerator UpdateWeaponPositionAndRotation(Transform weaponTransform, Quaternion initialRotation, Vector3 targetPosition, Rigidbody rb)
    {
        float duration = 1.0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {

            if (weaponTransform == null || weaponTransform.gameObject == null)
            {
                yield break;
            }

            weaponTransform.position = Vector3.Lerp(weaponTransform.position, targetPosition, elapsedTime / duration);

            Vector3 shootDirection;

            if (reticleTransform != null)
            {
                shootDirection = (reticleTransform.position - weaponTransform.position).normalized;
            }
            else
            {
                shootDirection = playerCamera.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(shootDirection) * Quaternion.AngleAxis(90, Vector3.right);

            weaponTransform.rotation = Quaternion.Slerp(weaponTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (weaponTransform != null)
        {
            weaponTransform.position = targetPosition;
        }

        //ensures direction is always updated.
        while (true)
        {
            if (weaponTransform == null || weaponTransform.gameObject == null)
            {
                yield break;
            }



            Vector3 shootDirection;

            if (reticleTransform != null)
            {
                shootDirection = (reticleTransform.position - weaponTransform.position).normalized;
            }
            else
            {
                shootDirection = playerCamera.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(shootDirection) * Quaternion.AngleAxis(90, Vector3.right);

            weaponTransform.rotation = Quaternion.Slerp(weaponTransform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            yield return null;
        }
    }

    public System.Collections.IEnumerator DestroyWeaponAfterDelay(GameObject weapon, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (weapon != null)
        {
            Destroy(weapon);
        }

    }
}
