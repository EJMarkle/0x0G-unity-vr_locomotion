using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Spawns wall segments at regular intervals behind the bike,
/// based on throttle input and distance traveled.
/// Supports both player and AI input via IBikeInput.
/// </summary>
public class LightWallTrail : MonoBehaviour
{
    [Header("Wall Segment Settings")]
    public GameObject wallSegmentPrefab;
    public float segmentLength = 0.2f;
    public int maxSegments = 100;

    [Tooltip("How long (seconds) before each segment is destroyed automatically")]
    public float segmentLifetime = 10f;

    [Header("Input Source")]
    public MonoBehaviour inputSource; // Must implement IBikeInput

    private IBikeInput input;
    private Vector3 lastSpawnPosition;
    private float distanceAccumulator = 0f;
    private Queue<GameObject> segmentQueue = new Queue<GameObject>();
    private bool wasThrottlingLastFrame = false;

    void Start()
    {
        // Validate input
        input = inputSource as IBikeInput;
        if (input == null)
        {
            Debug.LogError("Input source does not implement IBikeInput.");
            enabled = false;
            return;
        }

        // Initialize trail
        lastSpawnPosition = transform.position;
        SpawnWallSegment(lastSpawnPosition, transform.forward);
    }

    void Update()
    {
        if (input == null || wallSegmentPrefab == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 movementVector = currentPosition - lastSpawnPosition;
        float distanceMoved = movementVector.magnitude;

        // Sanity check: prevent invalid math
        if (!IsFiniteVector(movementVector) || distanceMoved < 0.0001f)
            return;

        distanceAccumulator += distanceMoved;

        bool isThrottling = input.throttleInput > 0.1f;

        // If throttle was off and is now resumed, reset spawn origin
        if (isThrottling && !wasThrottlingLastFrame)
        {
            lastSpawnPosition = transform.position;
            distanceAccumulator = 0f;
        }

        wasThrottlingLastFrame = isThrottling;

        if (!isThrottling)
            return;

        Vector3 direction = movementVector.normalized;

        // Sanity check direction
        if (!IsFiniteVector(direction))
            return;

        // Spawn segments as long as accumulated distance is sufficient
        while (distanceAccumulator >= segmentLength)
        {
            Vector3 spawnPosition = lastSpawnPosition + direction * segmentLength;

            if (!IsFiniteVector(spawnPosition))
                return;

            SpawnWallSegment(spawnPosition, direction);
            lastSpawnPosition = spawnPosition;
            distanceAccumulator -= segmentLength;
        }
    }

    private void SpawnWallSegment(Vector3 position, Vector3 forward)
    {
        Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
        GameObject segment = Instantiate(wallSegmentPrefab, position, rotation);
        segmentQueue.Enqueue(segment);

        // Destroy segment after lifetime expires
        Destroy(segment, segmentLifetime);

        if (segmentQueue.Count > maxSegments)
        {
            GameObject oldest = segmentQueue.Dequeue();
            Destroy(oldest);
        }
    }
    public void ResetTrail(Vector3 newStartPosition)
    {
        lastSpawnPosition = newStartPosition;
        distanceAccumulator = 0f;
    }


    /// <summary>
    /// Ensures the given vector has all finite components.
    /// Prevents NaN and infinity issues from breaking the trail logic.
    /// </summary>
    private bool IsFiniteVector(Vector3 v)
    {
        return float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z);
    }
}
