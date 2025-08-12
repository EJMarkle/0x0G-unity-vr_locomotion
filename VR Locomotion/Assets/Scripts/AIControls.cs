using UnityEngine;
using System.Collections;

/// <summary>
/// Handles AI movement logic for a bike, responding to current AIState.
/// Works with AIStateMachine to switch behaviors.
/// </summary>
public class AIControls : MonoBehaviour, IBikeInput
{
    [Header("Waypoints")]
    public Transform[] defaultWaypoints;
    public Transform startWaypoint;
    public float waypointRadius = 3f;
    public bool enteredStartPoint = false;

    [Header("Driving")]
    public float maxThrottle = 1f;
    public float maxSteeringAngle = 45f;
    public float turnSharpness = 1.5f;
    public float brakeDistance = 5f;

    [Header("Tilt")]
    public float tiltResponsiveness = 3f;

    [Header("Chase")]
    public float chaseRefreshInterval = 2f;

    [Header("Debug")]
    public bool drawDebug = true;

    // IBikeInput backing fields
    private float _throttleInput = 0f;
    private float _steering = 0f;
    private bool _isBraking = false;
    private float _tilt = 0f;

    public float throttleInput => _throttleInput;
    public float steering => _steering;
    public bool isBraking => _isBraking;
    public float tilt => _tilt;

    // Internal
    private int currentWaypointIndex = 0;
    private BikeMovement bikeMovement;

    // Wall avoidance
    private float avoidDirection = 0f; // -1 for left, 1 for right
    private bool currentlyAvoidingWall = false;

    // Chase logic
    private Transform chaseTarget;
    private float chaseTimer = 0f;

    void Start()
    {
        bikeMovement = GetComponent<BikeMovement>();
        if (bikeMovement == null)
        {
            Debug.LogError("BikeMovement not found on AIControls GameObject.");
            enabled = false;
            return;
        }

        if (defaultWaypoints == null || defaultWaypoints.Length == 0)
        {
            Debug.LogError("No defaultWaypoints assigned in AIControls.");
            enabled = false;
        }
    }

    // Called by AIStateMachine every frame
    public void FollowWaypointLogic()
    {
        if (defaultWaypoints.Length == 0) return;

        Vector3 currentPosition = transform.position;
        Transform currentWaypoint = defaultWaypoints[currentWaypointIndex];

        float distance = Vector3.Distance(currentPosition, currentWaypoint.position);
        if (distance < waypointRadius)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % defaultWaypoints.Length;
        }

        ApplySteeringAndThrottle(currentWaypoint.position);

        if (drawDebug)
        {
            Debug.DrawLine(currentPosition, currentWaypoint.position, Color.yellow);
        }
    }

    public void AvoidWallLogic()
    {
        if (!currentlyAvoidingWall)
        {
            avoidDirection = Random.value > 0.5f ? 1f : -1f;
            currentlyAvoidingWall = true;
        }

        _steering = avoidDirection * turnSharpness;
        _tilt = Mathf.Lerp(_tilt, _steering, Time.deltaTime * tiltResponsiveness);
        _throttleInput = 0f;
        _isBraking = true;

        if (drawDebug)
        {
            Debug.DrawRay(transform.position + Vector3.up * 0.5f, transform.forward * 5f, Color.red);
        }
    }

    public void EndWallAvoidance()
    {
        currentlyAvoidingWall = false;
        avoidDirection = 0f;
    }

    public void ChaseRacerLogic()
    {
        chaseTimer += Time.deltaTime;

        if (chaseTarget == null || chaseTimer >= chaseRefreshInterval)
        {
            FindNearestChaseTarget();
            chaseTimer = 0f;
        }

        if (chaseTarget == null)
            return;

        ApplySteeringAndThrottle(chaseTarget.position);

        if (drawDebug)
        {
            Debug.DrawLine(transform.position, chaseTarget.position, Color.red);
        }
    }

    private void FindNearestChaseTarget()
    {
        float closestDistance = float.MaxValue;
        Transform bestTarget = null;

        GameObject[] racers = GameObject.FindGameObjectsWithTag("Racer");

        foreach (GameObject racerObj in racers)
        {
            if (racerObj == this.gameObject)
                continue;

            Racer racer = racerObj.GetComponent<Racer>();
            if (racer == null || !racer.IsAlive() || racer.ChasePoint == null)
                continue;

            float distance = Vector3.Distance(transform.position, racer.ChasePoint.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = racer.ChasePoint;
            }
        }

        chaseTarget = bestTarget;
    }

    private void ApplySteeringAndThrottle(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;
        Vector3 toTarget = (targetPosition - currentPosition).normalized;
        toTarget.y = 0f;

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        float angle = Vector3.SignedAngle(forward, toTarget, Vector3.up);
        float normalizedAngle = Mathf.Clamp(angle / maxSteeringAngle, -1f, 1f);

        _steering = normalizedAngle * turnSharpness;
        _tilt = Mathf.Lerp(_tilt, normalizedAngle, Time.deltaTime * tiltResponsiveness);

        float distance = Vector3.Distance(currentPosition, targetPosition);
        if (distance < brakeDistance)
        {
            _throttleInput = Mathf.Lerp(0f, maxThrottle, distance / brakeDistance);
            _isBraking = true;
        }
        else
        {
            _throttleInput = maxThrottle;
            _isBraking = false;
        }
    }
    public void SetChaseTarget(Transform target)
    {
        chaseTarget = target;
        chaseTimer = 0f;
    }
    public bool NavigateToStartPoint()
    {
        if (enteredStartPoint) return true;
        if (startWaypoint == null) return true;

        float distance = Vector3.Distance(transform.position, startWaypoint.position);
        ApplySteeringAndThrottle(startWaypoint.position);

        if (drawDebug)
        {
            Debug.DrawLine(transform.position, startWaypoint.position, Color.cyan);
        }

        if (distance < waypointRadius)
        {
            enteredStartPoint = true;
            return true;
        }

        return false;
    }

    public void ResetInputs()
    {
        _throttleInput = 0f;
        _steering = 0f;
        _isBraking = true;
        _tilt = 0f;
        enteredStartPoint = false;
        
        // Reset internal state
        currentlyAvoidingWall = false;
        avoidDirection = 0f;
        chaseTarget = null;
        chaseTimer = 0f;
    }

}
