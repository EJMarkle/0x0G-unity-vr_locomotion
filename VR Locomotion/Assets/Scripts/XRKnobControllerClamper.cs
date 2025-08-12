using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.VRTemplate;

public class XRKnobControllerClamper : MonoBehaviour
{
    [Header("Clamping Settings")]
    [Tooltip("Should the controller position be clamped to the knob")]
    public bool clampPosition = true;
    
    [Tooltip("Should the controller rotation follow the knob rotation")]
    public bool clampRotation = false;
    
    [Tooltip("Offset from the knob center for controller positioning")]
    public Vector3 controllerOffset = Vector3.zero;
    
    [Tooltip("How smoothly to interpolate controller movement (0 = instant)")]
    [Range(0f, 1f)]
    public float clampSmoothness = 0.1f;

    private XRKnob knobComponent;
    private Transform attachPoint;
    private IXRSelectInteractor currentInteractor;
    private Vector3 originalControllerPosition;
    private Quaternion originalControllerRotation;
    private bool isGrabbed = false;
    
    // Performance optimization: cache controller transform
    private Transform cachedControllerTransform;
    private float lastUpdateTime;
    private const float UPDATE_FREQUENCY = 0.016f; // ~60fps for smoothness

    void Awake()
    {
        knobComponent = GetComponent<XRKnob>();
        if (knobComponent == null)
        {
            Debug.LogError("XRKnobControllerClamper requires an XRKnob component!");
            enabled = false;
            return;
        }

        // Use the knob's handle as the attach point, or this transform if no handle is set
        attachPoint = knobComponent.handle != null ? knobComponent.handle : transform;
    }

    void OnEnable()
    {
        if (knobComponent != null)
        {
            knobComponent.selectEntered.AddListener(OnGrabStarted);
            knobComponent.selectExited.AddListener(OnGrabEnded);
        }
    }

    void OnDisable()
    {
        if (knobComponent != null)
        {
            knobComponent.selectEntered.RemoveListener(OnGrabStarted);
            knobComponent.selectExited.RemoveListener(OnGrabEnded);
        }
    }

    void OnGrabStarted(SelectEnterEventArgs args)
    {
        currentInteractor = args.interactorObject;
        isGrabbed = true;

        // Cache controller transform for performance
        cachedControllerTransform = GetControllerTransform();
        if (cachedControllerTransform != null)
        {
            originalControllerPosition = cachedControllerTransform.position;
            originalControllerRotation = cachedControllerTransform.rotation;
        }
    }

    void OnGrabEnded(SelectExitEventArgs args)
    {
        currentInteractor = null;
        isGrabbed = false;
        cachedControllerTransform = null; // Clear cache
    }

    void Update()
    {
        // Limit update frequency for better performance
        if (isGrabbed && currentInteractor != null && clampPosition && 
            Time.time - lastUpdateTime >= UPDATE_FREQUENCY)
        {
            ClampControllerToKnob();
            lastUpdateTime = Time.time;
        }
    }

    void ClampControllerToKnob()
    {
        // Use cached transform for better performance
        if (cachedControllerTransform == null) return;

        // Calculate target position with offset
        Vector3 targetPosition = attachPoint.position + attachPoint.TransformDirection(controllerOffset);
        
        if (clampSmoothness > 0f)
        {
            // Use fixed deltaTime for consistent performance
            float deltaTime = UPDATE_FREQUENCY;
            cachedControllerTransform.position = Vector3.Lerp(
                cachedControllerTransform.position, 
                targetPosition, 
                deltaTime / clampSmoothness
            );
        }
        else
        {
            cachedControllerTransform.position = targetPosition;
        }

        // Handle rotation clamping if enabled
        if (clampRotation)
        {
            Quaternion targetRotation = attachPoint.rotation;
            
            if (clampSmoothness > 0f)
            {
                float deltaTime = UPDATE_FREQUENCY;
                cachedControllerTransform.rotation = Quaternion.Lerp(
                    cachedControllerTransform.rotation,
                    targetRotation,
                    deltaTime / clampSmoothness
                );
            }
            else
            {
                cachedControllerTransform.rotation = targetRotation;
            }
        }
    }

    Transform GetControllerTransform()
    {
        if (currentInteractor == null) return null;

        // Get the controller transform from the interactor
        var interactorTransform = currentInteractor.transform;
        
        // Try to find the actual controller transform (usually a parent)
        Transform controllerRoot = interactorTransform;
        while (controllerRoot.parent != null)
        {
            string parentName = controllerRoot.parent.name.ToLower();
            if (parentName.Contains("controller") && 
                (parentName.Contains("left") || parentName.Contains("right")))
            {
                controllerRoot = controllerRoot.parent;
                break;
            }
            controllerRoot = controllerRoot.parent;
        }

        return controllerRoot;
    }

    // Public method to restore original controller position (useful for debugging)
    public void RestoreControllerPosition()
    {
        if (cachedControllerTransform != null)
        {
            cachedControllerTransform.position = originalControllerPosition;
            cachedControllerTransform.rotation = originalControllerRotation;
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        if (attachPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 targetPos = attachPoint.position + attachPoint.TransformDirection(controllerOffset);
            Gizmos.DrawWireSphere(targetPos, 0.02f);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(attachPoint.position, targetPos);
        }
    }
}
