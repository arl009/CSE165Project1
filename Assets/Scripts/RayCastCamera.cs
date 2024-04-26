using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RayCastCamera : MonoBehaviour
{
    private GameObject lastHit = null;
    private XRRayInteractor rayInteractor;

    void Start()
    {
        // Get the XRRayInteractor component from this GameObject
        rayInteractor = GetComponent<XRRayInteractor>();
    }

    void Update()
    {
        // Check if the interactor has made a hit
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // Check if the hit object has the tag "Brick"
            if (hit.collider.tag == "Brick")
            {
                // Check if the current hit is different from the last hit
                if (lastHit != hit.collider.gameObject)
                {
                    // Notify the last hit object that it is no longer being hit
                    if (lastHit != null)
                    {
                        lastHit.SendMessage("OnRayExit", SendMessageOptions.DontRequireReceiver);
                    }
                    // Update last hit and notify the new hit object
                    lastHit = hit.collider.gameObject;
                    lastHit.SendMessage("OnRayEnter", SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (lastHit != null)
            {
                // Notify the last hit object that it is no longer being hit
                lastHit.SendMessage("OnRayExit", SendMessageOptions.DontRequireReceiver);
                lastHit = null;
            }
        }
        else if (lastHit != null)
        {
            // If there is no current hit, notify the last object it's no longer being hit
            lastHit.SendMessage("OnRayExit", SendMessageOptions.DontRequireReceiver);
            lastHit = null;
        }
    }
}
