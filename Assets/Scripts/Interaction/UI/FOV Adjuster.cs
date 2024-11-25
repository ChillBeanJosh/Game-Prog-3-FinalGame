using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FOVAdjuster : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine virtual camera
    public float minFOV = 45f;                     // Minimum Field of View
    public float maxFOV = 150f;                     // Maximum Field of View
    public float scrollSensitivity = 5f;          // Sensitivity of the scroll wheel

    private void Update()
    {
        // Check if the virtual camera is assigned
        if (virtualCamera == null)
        {
            Debug.LogError("Cinemachine Virtual Camera is not assigned!");
            return;
        }

        // Get the current FOV from the virtual camera's lens
        float currentFOV = virtualCamera.m_Lens.FieldOfView;

        // Adjust the FOV based on scroll wheel input
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollInput != 0)
        {
            currentFOV -= scrollInput * scrollSensitivity;
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV); // Clamp FOV to prevent going out of bounds

            // Set the updated FOV back to the virtual camera
            virtualCamera.m_Lens.FieldOfView = currentFOV;
        }
    }
}
