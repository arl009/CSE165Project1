using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SpawnPrefabOnButtonPress : MonoBehaviour
{
    public GameObject prefabToSpawnA; // Assign the prefab for the 'A' button in the Inspector
    public GameObject prefabToSpawnB; // Assign the prefab for the 'B' button in the Inspector
    public GameObject rightController; // Assign the right controller GameObject
    public GameObject leftController; // Assign the left controller GameObject
    private InputDevice rightDevice;
    private InputDevice leftDevice;
    private GameObject currentInstanceA;
    private GameObject currentInstanceB;
    private Rigidbody currentInstanceRigidbodyA;
    private Rigidbody currentInstanceRigidbodyB;
    private bool isButtonAHeld = false;
    private bool isButtonBHeld = false;

    void Start()
    {
        rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
    }

    void Update()
    {
        if (!rightDevice.isValid)
        {
            rightDevice = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        }
        if (!leftDevice.isValid)
        {
            leftDevice = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        }

        // Handle A button press
        rightDevice.TryGetFeatureValue(CommonUsages.primaryButton, out bool isAButtonPressed);
        HandleButtonPress(isAButtonPressed, ref isButtonAHeld, ref currentInstanceA, prefabToSpawnA, ref currentInstanceRigidbodyA);

        // Handle B button press
        rightDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool isBButtonPressed);
        HandleButtonPress(isBButtonPressed, ref isButtonBHeld, ref currentInstanceB, prefabToSpawnB, ref currentInstanceRigidbodyB);
    }

    private void HandleButtonPress(bool isButtonPressed, ref bool isButtonHeld, ref GameObject currentInstance, GameObject prefabToSpawn, ref Rigidbody currentInstanceRigidbody)
    {
        if (isButtonPressed)
        {
            if (!isButtonHeld)
            {
                Vector3 spawnPosition = rightController.transform.position + rightController.transform.forward * 1.5f;
                currentInstance = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
                currentInstanceRigidbody = currentInstance.GetComponent<Rigidbody>();
                if (currentInstanceRigidbody != null)
                {
                    currentInstanceRigidbody.isKinematic = true;  // Disable physics while positioning
                }
                isButtonHeld = true;
            }
            else
            {
                MoveAndRotateCurrentPrefabInstance(currentInstance);
            }
        }
        else if (isButtonHeld)
        {
            if (currentInstanceRigidbody != null)
            {
                currentInstanceRigidbody.isKinematic = false;  // Enable physics when the button is released
            }
            isButtonHeld = false;
        }
    }

    private void MoveAndRotateCurrentPrefabInstance(GameObject currentInstance)
    {
        if (currentInstance != null)
        {
            Vector3 newPosition = rightController.transform.position + rightController.transform.forward * 1.5f;
            currentInstance.transform.position = newPosition;
            Vector3 direction = (leftController.transform.position - rightController.transform.position).normalized;
            Quaternion rotation = Quaternion.Euler(0, 90, 0);
            Vector3 rotatedDirection = rotation * direction;
            currentInstance.transform.rotation = Quaternion.LookRotation(rotatedDirection);
        }
    }
}
