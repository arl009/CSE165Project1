using UnityEngine;
using System.Collections; // Required for using coroutines

public class HoverColorChange : MonoBehaviour
{
    public Color defaultColor = Color.gray;
    public Color hoverColor = Color.white;
    public Color finalColor = Color.yellow;
    private Renderer rend;
    private bool isHovering = false;
    private bool startLerp = false;  // Flag to control the start of the lerp
    private float transitionSpeed = 2f;  // Speed of the color transition
    private Coroutine lerpCoroutine;  // Reference to the coroutine for managing delays

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.color = defaultColor;
    }

    void Update()
    {
        if (startLerp)
        {
            rend.material.color = Color.Lerp(rend.material.color, finalColor, transitionSpeed * Time.deltaTime);
        }
    }

    // Called by raycast when it hits the object
    void OnRayEnter()
    {
        isHovering = true;
        rend.material.color = hoverColor;  // Change to hover color immediately
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(EnableLerpAfterDelay());
    }

    // Called by raycast when it no longer hits the object
    void OnRayExit()
    {
        isHovering = false;
        startLerp = false;
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        rend.material.color = defaultColor;  // Revert to default color immediately
    }

    IEnumerator EnableLerpAfterDelay()
    {
        yield return new WaitForSeconds(1);
        if (isHovering)  // Only start lerp if still hovering after 1 second
        {
            startLerp = true;
        }
    }
}
