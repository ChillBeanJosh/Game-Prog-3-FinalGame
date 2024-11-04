using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponCollector : MonoBehaviour
{
    private const int maxWeapons = 2;
    public int currentWeapons = 0;

    //weapon positions.
    public Transform weaponOnLeft;
    public Transform weaponOnRight;
    public Camera playerCamera;

    public float throwForce = 10f;
    public float rotationSpeed = 5f;
    public float destructionDelay = 5f;

    public Vector3 offset = Vector3.zero;

    public Transform reticleTransform;

    //issue: Weapon sometimes appear as there is ONLY 1, but the stack on the same side.
    public void CollectWeapon(GameObject weapon)
    {
        if (currentWeapons < maxWeapons)
        {
            Vector3 targetPosition = (currentWeapons == 0) ? weaponOnLeft.position : weaponOnRight.position;
            weapon.transform.SetParent(transform);
            weapon.transform.position = targetPosition + offset;

            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;

            currentWeapons++;
        }
    }

    public IEnumerator ThrowWeapon(GameObject weapon)
    {
        if (weapon.transform.IsChildOf(transform))
        {
            // Detach weapon from player
            weapon.transform.SetParent(null);

            Rigidbody rb = weapon.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;
            }

            Vector3 shootDirection;

            if (reticleTransform != null)
            {
                shootDirection = (reticleTransform.position - weapon.transform.position).normalized;
            }
            else
            {
                shootDirection = playerCamera.transform.forward;
            }

            Quaternion targetRotation = Quaternion.LookRotation(shootDirection) * Quaternion.AngleAxis(90, Vector3.right);
            float elapsedTime = 0f;
            float duration = 0.3f; //time until sword is thrown.

            while (elapsedTime < duration)
            {
                weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, targetRotation, elapsedTime / duration * rotationSpeed);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            if (rb != null)
            {
                weapon.transform.rotation = targetRotation;

                rb.AddForce(shootDirection * throwForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

                rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
            }

            StartCoroutine(DestroyWeaponAfterDelay(weapon, destructionDelay));

            currentWeapons--;
            Debug.Log("YOU NOW HAVE: " + currentWeapons + "!");
        }
    }

    public IEnumerator DestroyWeaponAfterDelay(GameObject weapon, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(weapon);
    }

    public Vector3 CalculateThrowDirection(Vector3 weaponPosition)
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            return (hit.point - weaponPosition).normalized;
        }
        return playerCamera.transform.forward;
    }

    public int GetWeaponCount()
    {
        return currentWeapons;
    }
}
