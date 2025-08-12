using UnityEngine;
using System.Collections;

public class AIStateMachine : MonoBehaviour
{
    public enum AIState
    {
        Start,
        FollowPath,
        AvoidWall,
        Chase
    }

    public AIState currentState = AIState.Start;

    private AIControls aiControls;
    private Coroutine stateCoroutine;

    [Header("Wall Avoidance")]
    public float wallDetectionDistance = 5f;
    public LayerMask lightWallLayer;

    [Header("Chase Settings")]
    public float chaseTriggerRange = 20f;

    void Awake()
    {
        aiControls = GetComponent<AIControls>();
        if (aiControls == null)
        {
            Debug.LogError("AIControls not found on AIStateMachine GameObject.");
            enabled = false;
        }
    }

    void Update()
    {
        // Force AI to remain in Start state if it hasn't reached the start point
        if (!aiControls.enteredStartPoint)
        {
            currentState = AIState.Start;
        }

        if (currentState == AIState.AvoidWall)
        {
            if (CheckForWalls()) return;
            currentState = AIState.FollowPath;
            aiControls.EndWallAvoidance();
        }

        switch (currentState)
        {
            case AIState.Start:
                if (aiControls.NavigateToStartPoint())
                {
                    currentState = AIState.FollowPath;
                }
                break;

            case AIState.Chase:
                if (!CheckForWalls() && TryFindChaseTarget(out Racer target))
                {
                    aiControls.SetChaseTarget(target.ChasePoint);
                    aiControls.ChaseRacerLogic();
                }
                else
                {
                    currentState = AIState.FollowPath;
                }
                break;

            case AIState.FollowPath:
                if (CheckForWalls()) return;

                if (TryFindChaseTarget(out Racer chaseTarget))
                {
                    currentState = AIState.Chase;
                    aiControls.SetChaseTarget(chaseTarget.ChasePoint);
                    aiControls.ChaseRacerLogic();
                }
                else
                {
                    aiControls.FollowWaypointLogic();
                }
                break;
        }
    }

    private bool CheckForWalls()
    {
        Vector3 origin = transform.position + Vector3.down * 0.5f;
        Vector3 direction = transform.forward;
        Debug.DrawRay(origin, direction * wallDetectionDistance, Color.green, 0.1f);

        if (Physics.Raycast(origin, direction, wallDetectionDistance, lightWallLayer))
        {
            Debug.DrawRay(origin, direction * wallDetectionDistance, Color.red);
            if (currentState != AIState.AvoidWall)
            {
                currentState = AIState.AvoidWall;
                if (stateCoroutine != null) StopCoroutine(stateCoroutine);
                stateCoroutine = StartCoroutine(ReturnToPreviousState());
                aiControls.AvoidWallLogic();
            }
            return true;
        }

        return false;
    }

    private IEnumerator ReturnToPreviousState()
    {
        yield return new WaitForSeconds(1.0f);
        aiControls.EndWallAvoidance();
        currentState = AIState.FollowPath;
        stateCoroutine = null;
    }

    private bool TryFindChaseTarget(out Racer bestCandidate)
    {
        bestCandidate = null;
        GameObject[] racers = GameObject.FindGameObjectsWithTag("Racer");

        float closestDist = float.MaxValue;

        foreach (GameObject obj in racers)
        {
            if (obj == this.gameObject) continue;

            Racer r = obj.GetComponent<Racer>();
            if (r == null || !r.IsAlive() || r.ChasePoint == null) continue;

            float dist = Vector3.Distance(transform.position, r.transform.position);
            if (dist < closestDist && dist <= chaseTriggerRange)
            {
                closestDist = dist;
                bestCandidate = r;
            }
        }

        return bestCandidate != null;
    }

    public void ResetAIState()
    {
        currentState = AIState.Start;
    }
}
