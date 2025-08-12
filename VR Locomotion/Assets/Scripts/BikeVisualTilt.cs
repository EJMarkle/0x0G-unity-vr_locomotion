using UnityEngine;

public class BikeVisualTilt : MonoBehaviour
{
    [Header("Input Source")]
    public MonoBehaviour inputSource; // Can be BikeControls or AIControls

    [Header("Tilt Settings")]
    public float maxTiltAngle = 30f;    // Maximum Z-axis rotation in degrees
    public float tiltSmoothing = 5f;    // Smoothing speed for interpolation

    private IBikeInput input;
    private float currentTilt = 0f;

    void Start()
    {
        input = inputSource as IBikeInput;

        if (input == null)
        {
            Debug.LogError("Assigned input source does not implement IBikeInput.");
        }
    }

    void Update()
    {
        if (input == null)
            return;

        // Get the target tilt based on input value
        float targetTilt = input.tilt * maxTiltAngle;

        // Smoothly interpolate current tilt toward target
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSmoothing);

        // Apply Z-axis rotation (banking effect)
        transform.localRotation = Quaternion.Euler(0f, 0f, -currentTilt);
    }
}
