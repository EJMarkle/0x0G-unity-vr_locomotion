using UnityEngine;

public class MenuAudio : MonoBehaviour
{
    public AudioSource idle;
    public AudioSource select;
    public BikeControls bikeControls;
    private float throttle;
    private float steering;
    
    [Header("Pitch Settings")]
    public float minPitch = 1.0f;
    public float maxPitch = 2.0f;
    private bool playSelect;
    private bool hasSteered;

    void Start()
    {
        if (!idle.isPlaying)
        {
            idle.loop = true;
            idle.Play();
        }
    }
    void Update()
    {
        throttle = bikeControls.throttleInput;
        steering = bikeControls.steering;

    // Play select sound once on new steering input
    if (!hasSteered && steering != 0)
    {
        select.Play();
        hasSteered = true;
    }

    // Reset when steering returns to neutral
    if (steering == 0)
    {
        hasSteered = false;
    }

        float targetPitch = Mathf.Lerp(minPitch, maxPitch, throttle);

        idle.pitch = Mathf.Lerp(idle.pitch, targetPitch, Time.deltaTime * 5f);
    }
}
