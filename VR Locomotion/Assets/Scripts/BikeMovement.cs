using System;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class BikeMovement : MonoBehaviour
{
    [Header("Bike References")]
    public Transform forwardSource;
    public MonoBehaviour inputSource; 
    private IBikeInput input;

    [Range(0, 1)]
    public float throttle;
    [Range(-1, 1)]
    public float tilt;
    [Range(-1, 1)]
    public float steering;
    public float turnSteer = 0.5f;
    public bool isBraking;
    public float speed;


    [Header("Engine Settings")]
    public float maxForce = 1000f;
    public float brakeStrength = 500f;

    [Header("Steering Settings")]
    public float strafeForce = 500f;
    public float rotationSpeed = 3f;

    [Header("Hover Settings")]
    public Transform[] hoverPoints;
    public float hoverHeight = 3f;
    public float hoverForce = 1000f;
    public float damping = 5f;

    private Rigidbody rb;

    [SerializeField] float maxSpeed = 50f; // Top speed you're aiming for   
    [SerializeField] AnimationCurve accelerationCurve; 


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        input = inputSource as IBikeInput;
        if (input == null)
        {
            Debug.LogError("Assigned inputSource does not implement IBikeInput.");
        }
    }
    void Update()
    {
        if (input == null) return;
        throttle = input.throttleInput;
        tilt = input.tilt;
        steering = input.steering;
        isBraking = input.isBraking;
    }

    void FixedUpdate()
    {
        CheckSpeed();
        ApplyHoverForces();
        //TiltBike();

        float velocityThreshold = 0.1f;

        if (rb.linearVelocity.magnitude > velocityThreshold)
        {
            Rotate();
            Strafe();
        }

        if (throttle > 0f)
        {
            Accelerate();
        }

        if (isBraking)
        {
            Brake();
        }
    }

    private void CheckSpeed()
    {
        // Get the full velocity
        Vector3 velocity = rb.linearVelocity;

        // Optionally ignore vertical movement if you're focusing on horizontal motion
        Vector3 horizontalVelocity = velocity;
        horizontalVelocity.y = 0f;

        // Get the bike's current forward direction on the horizontal plane
        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Forward speed: how fast we're moving in the direction we're facing
        float forwardSpeed = Vector3.Dot(horizontalVelocity, forward);

        // Total speed: overall velocity magnitude
        float totalSpeed = velocity.magnitude;

        // Update public field(s)
        speed = forwardSpeed;
    }

    void ApplyHoverForces()
    {
        foreach (var point in hoverPoints)
        {
            Ray ray = new Ray(point.position, -Vector3.up);
            RaycastHit hit;

            bool grounded = Physics.Raycast(ray, out hit, hoverHeight);
            if (grounded)
            {
                float distance = hit.distance;
                float forcePercent = 1f - (distance / hoverHeight);
                float upwardForce = forcePercent * hoverForce;
                rb.AddForceAtPosition(Vector3.up * upwardForce, point.position);
            }

            // Apply damping regardless of ground detection
            Vector3 pointVelocity = rb.GetPointVelocity(point.position);
            Vector3 dampingForce = -pointVelocity * damping;
            rb.AddForceAtPosition(dampingForce, point.position);
        }

    }

    void Accelerate()
    {
        Vector3 forwardDirection = transform.forward;
        forwardDirection.y = 0f;
        forwardDirection.Normalize();

        // Calculate the normalized speed (0 to 1)
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);

        //  Option A: simple logarithmic falloff
        float speedMultiplier = 1;


        // Option B: use an AnimationCurve for custom shaping (drag curve into inspector)
        if (accelerationCurve != null && accelerationCurve.length > 0)
        {
            speedMultiplier = accelerationCurve.Evaluate(speedRatio);
        }

        // Apply scaled force
        Vector3 force = forwardDirection * throttle * maxForce * speedMultiplier;
        rb.AddForce(force, ForceMode.Force);
    }

    public void Brake()
    {
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 brakeForce = -rb.linearVelocity.normalized * brakeStrength;
            rb.AddForce(brakeForce, ForceMode.Force);
        }
    }

    void Rotate()
    {
        // Rotate the Rigidbody using physics-friendly method
        float rotationAmount = tilt * rotationSpeed * Time.fixedDeltaTime;
        Quaternion currentRotation = rb.rotation;
        Quaternion deltaRotation = Quaternion.Euler(rotationAmount, 0f, 0f);
        rb.MoveRotation(currentRotation * deltaRotation);
    }
    void Strafe()
    {
        // Use the forward direction the bike is facing
        Vector3 forward = rb.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        // Calculate dynamic right direction (perpendicular to forward and up)
        Vector3 right = Vector3.Cross(Vector3.up, forward);
        right.Normalize();

        // Apply strafe force to the right (inverted if needed)
        Vector3 strafe = right * steering * strafeForce;
        rb.AddForce(strafe, ForceMode.Force);

        // Slight rotation
        float turn = steering * turnSteer;
        Quaternion rollRotation = Quaternion.Euler(turn, 0f, 0f);
        rb.MoveRotation(rb.rotation * rollRotation);

        // Draw the actual strafe direction
        //Debug.DrawRay(transform.position, strafe.normalized * 2f, Color.cyan);
    }

    void LateUpdate()
    {
        // Optional: keep forwardSource in sync with Rigidbody rotation
        if (forwardSource != null && forwardSource != transform)
        {
            forwardSource.rotation = rb.rotation;
        }
    }
}
