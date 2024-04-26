using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class XRInputExample : MonoBehaviour
{
    public XRNode teleportInputSource; // Typically the left controller
    public XRNode rotationInputSource; // Typically the right controller
    private InputDevice teleportDevice;
    private InputDevice rotationDevice;
    public XRRayInteractor rayInteractor; // Assign the XRRayInteractor component from the teleport controller
    public Transform player; // Assign your player's transform here
    public GameObject reticlePrefab; // Drag your sphere prefab here in the inspector
    public float rotationAngle = 45.0f; // The angle to rotate per snap

    private GameObject reticleInstance;
    private bool wasButtonPressedLastFrame = false; // Track the teleport button press state across frames
    private bool wasJoystickMovedLastFrame = false; // Track the joystick movement state across frames

    void Start()
    {
        teleportDevice = InputDevices.GetDeviceAtXRNode(teleportInputSource);
        rotationDevice = InputDevices.GetDeviceAtXRNode(rotationInputSource);
        if (reticlePrefab != null)
        {
            reticleInstance = Instantiate(reticlePrefab);
            reticleInstance.SetActive(false); // Start with the reticle hidden
        }
    }

    void Update()
    {
        if (!teleportDevice.isValid)
        {
            teleportDevice = InputDevices.GetDeviceAtXRNode(teleportInputSource);
        }
        if (!rotationDevice.isValid)
        {
            rotationDevice = InputDevices.GetDeviceAtXRNode(rotationInputSource);
        }

        HandleTeleportation();
        HandleSnapRotation();
        UpdateReticle();
    }

    void HandleTeleportation()
    {
        bool isButtonPressed = teleportDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonPressed) && primaryButtonPressed;

        if (isButtonPressed && !wasButtonPressedLastFrame)
        {
            if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
            {
                // Check if the hit object is on the "Teleport" layer
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Teleport"))
                {
                    // Teleport the player to the hit point if the primary button is pressed just once
                    player.position = hit.point;
                    Debug.Log("Teleported to: " + hit.point);
                }
            }
        }

        wasButtonPressedLastFrame = isButtonPressed;
    }

    void HandleSnapRotation()
    {
        if (rotationDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axisValue))
        {
            bool isJoystickMoved = Mathf.Abs(axisValue.x) > 0.5f;
            if (isJoystickMoved && !wasJoystickMovedLastFrame)
            {
                float rotationDirection = Mathf.Sign(axisValue.x);
                player.Rotate(0, rotationAngle * rotationDirection, 0);
            }
            wasJoystickMovedLastFrame = isJoystickMoved;
        }
    }

    void UpdateReticle()
    {
        // Attempt to get a raycast hit from the ray interactor
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Check if the hit object is on the "Teleport" layer
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Teleport"))
            {
                // Activate the reticle and position it at the hit point, orienting to match the hit surface
                if (reticleInstance != null)
                {
                    reticleInstance.SetActive(true);
                    reticleInstance.transform.position = hit.point;
                    reticleInstance.transform.up = hit.normal;
                }
            }
            else
            {
                // Deactivate the reticle if the hit object is not on the "Teleport" layer
                if (reticleInstance != null)
                {
                    reticleInstance.SetActive(false);
                }
            }
        }
        else
        {
            // Deactivate the reticle if no object is hit
            if (reticleInstance != null)
            {
                reticleInstance.SetActive(false);
            }
        }
    }
}
