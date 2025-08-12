using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class playerUI : MonoBehaviour
{
    public TMP_Text speedometer;
    public TMP_Text lives;
    public GameObject gameOverMessage;
    public GameObject deathMessage;
    public TextMeshProUGUI deathMenuText;
    public TextMeshProUGUI deathPlayText;
    public GameObject gameUI;
    public TMP_Text gameOverText;
    public TextMeshProUGUI againText;
    public TextMeshProUGUI menuText;
    public GameManager gameManager;
    public BikeMovement bikeMovement;
    public BikeControls bikeControls;
    public Racer racer;
    
    [Header("Input Delay Settings")]
    public float inputDelayTime = 1.5f; // Configurable delay time
    
    private float steering;
    private float throttle;
    private float gameOverInputTimer = 0f;
    private float deathInputTimer = 0f;
    private bool gameOverInputEnabled = false;
    private bool deathInputEnabled = false;

    void Update()
    {
        speedometer.text = bikeMovement.speed.ToString("F1");
        lives.text = racer.lives.ToString();
        steering = bikeControls.steering;
        throttle = bikeControls.throttleInput;

        if (gameManager.playerDead)
        {
            gameManager.FreezeAllRacers();
            gameUI.SetActive(false);
            deathMessage.SetActive(true);
            StartCoroutine(FlashText(deathMenuText));
            StartCoroutine(FlashText(deathPlayText));

            // Handle input delay for death menu
            if (!deathInputEnabled)
            {
                deathInputTimer += Time.unscaledDeltaTime;
                if (deathInputTimer >= inputDelayTime)
                {
                    deathInputEnabled = true;
                }
            }
            else
            {
                // Only check input after delay
                if (steering != 0f)
                {
                    SceneManager.LoadScene("Menu");
                }
                if (throttle > 0.1f)
                {
                    SceneManager.LoadScene("Arena");
                }
            }
        }
        else
        {
            // Reset death input state when not in death menu
            deathInputTimer = 0f;
            deathInputEnabled = false;
        }

        if (gameManager.gameOver)
        {
            gameUI.SetActive(false);
            gameOverMessage.SetActive(true);
            StartCoroutine(FlashText(againText));
            StartCoroutine(FlashText(menuText));
            gameOverText.text = "Game over " + gameManager.winner.racerName + " won!";

            // Handle input delay for game over menu
            if (!gameOverInputEnabled)
            {
                gameOverInputTimer += Time.unscaledDeltaTime;
                if (gameOverInputTimer >= inputDelayTime)
                {
                    gameOverInputEnabled = true;
                }
            }
            else
            {
                // Only check input after delay
                if (steering != 0f)
                {
                    SceneManager.LoadScene("Menu");
                }
                if (throttle > 0.1f)
                {
                    SceneManager.LoadScene("Arena");
                }
            }
        }
        else
        {
            // Reset game over input state when not in game over menu
            gameOverInputTimer = 0f;
            gameOverInputEnabled = false;
        }
    }

    private IEnumerator FlashText(TextMeshProUGUI tmp)
    {
        float duration = 1f;
        float t = 0f;
        bool fadingOut = true;
        Color baseColor = tmp.color;

        while (true)
        {
            t += Time.unscaledDeltaTime / duration; // Use unscaledDeltaTime for proper flashing when time is stopped
            float alpha = fadingOut ? Mathf.Lerp(1f, 0f, t) : Mathf.Lerp(0f, 1f, t);

            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            if (t >= 1f)
            {
                t = 0f;
                fadingOut = !fadingOut;
            }

            yield return null;
        }
    }
}
