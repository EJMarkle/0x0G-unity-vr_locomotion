using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks racers, manages lives, handles eliminations and victory.
/// Attach this to a persistent object in the scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Racers")]
    public List<Racer> racers = new List<Racer>();

    [Header("Game State")]
    public bool gameOver = false;
    public Racer winner;
    public bool playerDead = false;

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        ResetAllRacers();
    }

    void Start()
    {
        RegisterAllRacers();
    }

    void RegisterAllRacers()
    {
        racers.Clear();
        Racer[] foundRacers = FindObjectsOfType<Racer>();

        foreach (Racer racer in foundRacers)
        {
            racers.Add(racer);
            Debug.Log($"Registered racer: {racer.racerName} ({racer.lives} lives)");
        }
    }

    public void OnRacerEliminated(Racer racer)
    {
        Debug.Log($"{racer.racerName} has been eliminated.");

        // Check how many racers remain alive
        List<Racer> alive = racers.FindAll(r => r.IsAlive());

        if (alive.Count == 1)
        {
            winner = alive[0];
            Debug.Log($"Game Over! Winner: {winner.racerName}");
            gameOver = true;
            EndGame();
        }
        else if (alive.Count == 0)
        {
            Debug.Log("Game Over! No winner (draw or all eliminated).");
            gameOver = true;
            EndGame();
        }
    }

    void EndGame()
    {
        // Optional: freeze time, show UI, return to menu
        //Time.timeScale = 0f;
        FreezeAllRacers();
    }

    public void ResetAllRacers()
    {
        foreach (var racer in racers)
        {
            racer.ResetRacer();
        }

        gameOver = false;
        winner = null;
        Time.timeScale = 1f;
        Debug.Log("Racers reset.");
    }
    public void FreezeAllRacers()
    {
        foreach (var racer in racers)
        {
            MonoBehaviour[] movementScripts = racer.GetComponents<MonoBehaviour>();

            foreach (var script in movementScripts)
            {
                if (script is AIControls || script is BikeMovement)
                {
                    script.enabled = false;
                }
            }

            FreezeRigidbody(racer.GetComponent<Rigidbody>());
        }
    }

    void FreezeRigidbody(Rigidbody rb)
    {
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }


}
