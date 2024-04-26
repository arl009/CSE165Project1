using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabObjectWithTrigger : MonoBehaviour
{
    public XRRayInteractor rayInteractor; // Assign this in the Inspector

    private InputDevice rightDevice;
    private InputDevice leftDevice;
    private GameObject grabbedObject;
    public GameObject rightController; // Assign the right controller GameObject
    public GameObject leftController; // Assign the left controller GameObject
    private Rigidbody grabbedObjectRigidbody; // To hold the Rigidbody of the grabbed object
    private bool isRightButtonHeld = false;
    private bool isLeftButtonHeld = false;
    private bool isLeftGripHeld = false;
    private float initialDistance;
    private Vector3 initialScale;
    private Quaternion initialControllerOrientation;
    private Quaternion initialObjectRotation;
    private Vector3 initialDirection;
    private Vector3 initialUp;

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

        // Check for trigger and grip button presses
        rightDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isRightTriggerPressed);
        leftDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool isLeftGripPressed);
        leftDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool isLeftTriggerPressed);

        // Handle grabbing and moving with the right controller
        if (isRightTriggerPressed)
        {
            if (!isRightButtonHeld)
            {
                TryGrabObject();
                isRightButtonHeld = true;
            }
            else
            {
                MoveGrabbedObject();
            }
        }
        else if (isRightButtonHeld)
        {
            ReleaseObject();
            isRightButtonHeld = false;
        }

        // Handle rotating with the left controller trigger
        if (isLeftTriggerPressed)
        {
            if (!isLeftButtonHeld)
            {
                StartRotation();
                isLeftButtonHeld = true;
            }
            else if (grabbedObject != null)
            {
                RotateGrabbedObject();
            }
        }
        else if (isLeftButtonHeld)
        {
            isLeftButtonHeld = false;
        }

        // Handle scaling with the left controller grip
        if (isLeftGripPressed)
        {
            if (!isLeftGripHeld)
            {
                StartScaling();
                isLeftGripHeld = true;
            }
            else if (grabbedObject != null)
            {
                ScaleObject();
            }
        }
        else if (isLeftGripHeld)
        {
            isLeftGripHeld = false;
        }
    }

    private void TryGrabObject()
    {
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            grabbedObject = hit.collider.gameObject;
            grabbedObjectRigidbody = grabbedObject.GetComponent<Rigidbody>();
            if (grabbedObjectRigidbody != null)
            {
                grabbedObjectRigidbody.isKinematic = true;
            }
        }
    }

    private void MoveGrabbedObject()
    {
        if (grabbedObject != null)
        {
            Vector3 newPosition = rightController.transform.position + rightController.transform.forward * 1.5f;
            grabbedObject.transform.position = newPosition;
        }
    }

    private void StartRotation()
    {
        if (grabbedObject != null)
        {
            Vector3 initialDirection = (leftController.transform.position - rightController.transform.position).normalized;
            // Capture the initial world space orientation of the controllers
            initialControllerOrientation = Quaternion.LookRotation(initialDirection, Vector3.up);
            // Capture the initial rotation of the object in world space
            initialObjectRotation = grabbedObject.transform.rotation;
        }
    }

    private void RotateGrabbedObject()
    {
        if (grabbedObject != null)
        {
            Vector3 currentDirection = (leftController.transform.position - rightController.transform.position).normalized;
            Quaternion currentControllerOrientation = Quaternion.LookRotation(currentDirection, Vector3.up);

            // Calculate the rotation offset from the initial controller orientation to the current
            Quaternion rotationOffset = Quaternion.Inverse(initialControllerOrientation) * currentControllerOrientation;

            // Apply this rotation offset to the initial object rotation directly
            grabbedObject.transform.rotation = initialObjectRotation * rotationOffset;
        }
    }
    private void StartScaling()
    {
        if (grabbedObject != null)
        {
            initialDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            initialScale = grabbedObject.transform.localScale;
        }
    }

    private void ScaleObject()
    {
        if (grabbedObject != null)
        {
            float currentDistance = Vector3.Distance(leftController.transform.position, rightController.transform.position);
            float scaleMultiplier = currentDistance / initialDistance;
            grabbedObject.transform.localScale = initialScale * scaleMultiplier;
        }
    }

    private void ReleaseObject()
    {
        if (grabbedObjectRigidbody != null)
        {
            grabbedObjectRigidbody.isKinematic = false;
        }
        grabbedObject = null;
    }
}
