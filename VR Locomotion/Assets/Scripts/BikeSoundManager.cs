using UnityEngine;

public class BikeSoundManager : MonoBehaviour
{
    [Header("References")]
    public BikeMovement bikeMovement;
    public AudioSource idleSound;
    public AudioSource deathSound;

    [Header("Pitch Settings")]
    public float minPitch = 1.0f;
    public float maxPitch = 2.0f;
    public float maxSpeed = 100f;

    private float speed;

    void Start()
    {
        if (!idleSound.isPlaying)
        {
            idleSound.loop = true;
            idleSound.Play();
        }
    }

    void Update()
    {
        speed = bikeMovement.speed;

        // Calculate pitch based on speed
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, speed / maxSpeed);

        // Optional: Smooth pitch change to avoid jittery sound
        idleSound.pitch = Mathf.Lerp(idleSound.pitch, targetPitch, Time.deltaTime * 5f);
    }
}
