using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Racer : MonoBehaviour
{
    public BikeSoundManager bikeSoundManager;
    public AIStateMachine aIStateMachine;
    public AIControls aIControls;
    public GameManager gameManager;
    public BikeMovement bikeMovement;

    [Header("Racer Settings")]
    public string racerName = "Racer";
    public int lives = 3;
    public Transform respawnPoint;

    [Header("Chase Target")]
    [Tooltip("A point in front of the racer for AI to target while chasing.")]
    public Transform ChasePoint;

    [Header("Wall Trail")]
    public LightWallTrail wallTrail;

    [Header("Invincibility Settings")]
    public float invincibilityDuration = 1.5f;

    // Internal state
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private bool isInvincible = false;

    void Awake()
    {
        // Save fallback position/rotation before any potential modifications
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void Start()
    {
        if (ChasePoint == null)
        {
            Debug.LogWarning($"{racerName} is missing a ChasePoint reference!");
        }
    }

    public void LoseLife()
    {
        bikeSoundManager.deathSound.Play();
        if (isInvincible) return;

        lives--;
        Debug.Log($"{racerName} lost a life! Lives remaining: {lives}");

        if (lives <= 0)
        {
            Eliminate();
        }
        else
        {
            Respawn();
            StartCoroutine(InvincibilityTimer());
        }
    }

    private void Respawn()
    {
        // Reset physics safely
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // Reset position and rotation
        if (respawnPoint != null)
        {
            transform.position = respawnPoint.position;
            transform.rotation = respawnPoint.rotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        // Re-enable physics after repositioning
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        // Reset AI inputs and state
        if (aIControls != null)
        {
            aIControls.ResetInputs();
        }

        if (aIStateMachine != null)
        {
            aIStateMachine.ResetAIState();
        }

        // Reset trail
        wallTrail?.ResetTrail(transform.position);

        // Ensure the object is active
        gameObject.SetActive(true);
    }

    private void Eliminate()
    {
        Debug.Log($"{racerName} has been eliminated!");

        if (gameManager != null)
        {
            gameManager.OnRacerEliminated(this);
        }

        if (racerName == "You")
        {
            gameManager.playerDead = true;
            return;
        }

        gameObject.SetActive(false);
    }

    public void ResetRacer()
    {
        lives = 3;
        Respawn();
        gameObject.SetActive(true);
        Debug.Log($"{racerName} has been reset with {lives} lives.");
    }

    public bool IsAlive()
    {
        return lives > 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LightWall"))
        {
            LoseLife();
        }
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }
}
