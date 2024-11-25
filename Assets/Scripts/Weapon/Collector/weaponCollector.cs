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

    public Vector3 offset = Vector3.zero;

    public Transform reticleTransform;

    private bool canCollect = true;  
    private float collectCooldown = 1f;

    // Collect a weapon
    public void CollectWeapon(GameObject weapon)
    {
        if (currentWeapons < maxWeapons && canCollect)  
        {
            Vector3 targetPosition = (currentWeapons == 0) ? weaponOnLeft.position : weaponOnRight.position;
            weapon.transform.SetParent(transform);
            weapon.transform.position = targetPosition + offset;

            weapon.transform.localRotation = Quaternion.identity;
            weapon.transform.localScale = Vector3.one;

            currentWeapons++;
            Debug.Log("Weapon Collected! Total: " + currentWeapons);
        }
        else
        {
            Debug.Log("Cannot collect weapon right now.");
        }
    }

    public IEnumerator ThrowWeapon(GameObject weapon)
    {
        if (weapon == null || !weapon.transform.IsChildOf(transform))
        {
            yield break;  
        }

        StartCoroutine(CollectCooldown());

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
        float duration = 0.3f;  // Time until sword is thrown

        // Rotate the weapon smoothly toward the target direction
        while (elapsedTime < duration)
        {
            if (weapon == null) yield break;  
            weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, targetRotation, elapsedTime / duration * rotationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (rb != null)
        {
            if (weapon == null) yield break;  
            weapon.transform.rotation = targetRotation;

            rb.AddForce(shootDirection * throwForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

            rb.constraints = RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationX;
        }

        currentWeapons--;
        Debug.Log("You now have: " + currentWeapons + " weapons!");
    }

    private IEnumerator CollectCooldown()
    {
        canCollect = false; 
        Debug.Log("Weapon collection disabled for " + collectCooldown + " seconds.");
        yield return new WaitForSeconds(collectCooldown); 
        canCollect = true;  
        Debug.Log("Weapon collection enabled.");
    }

    // Method to decrement weapon count when the weapon is destroyed by explosion
    public void DecrementWeaponCount()
    {
        if (currentWeapons > 0)
        {
            currentWeapons--;
            Debug.Log("Weapon count decremented due to explosion. Current count: " + currentWeapons);
        }
    }

    // Calculate direction for the throw (not used in this example but could be useful elsewhere)
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

    // Get the number of collected weapons
    public int GetWeaponCount()
    {
        return currentWeapons;
    }
}
