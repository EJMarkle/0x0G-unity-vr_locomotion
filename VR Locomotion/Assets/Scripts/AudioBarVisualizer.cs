using UnityEngine;

public class XAxisBarOscillator : MonoBehaviour
{
    public BikeMovement bikeMovement;
    public Transform[] bars;
    public float[] speeds;

    private Vector3[] basePositions;

    // Output range
    private const float targetMin = 0f;
    private const float targetMax = 0.007f;

    // Input range
    private const float sourceMin = 0f;
    private const float sourceMax = 102.9f;

    [Range(1f, 5f)]
    public float exponent = 3f;  // Controls the curve shape; 3 is a good start

    void Start()
    {
        basePositions = new Vector3[bars.Length];
        for (int i = 0; i < bars.Length; i++)
        {
            basePositions[i] = bars[i].localPosition;
        }
    }

    void Update()
    {
        float bikeSpeed = bikeMovement.speed;
        float normalizedSpeed = Mathf.InverseLerp(sourceMin, sourceMax, bikeSpeed);

        // Apply exponential curve to normalized speed for logarithmic-like ramp
        float curvedSpeed = Mathf.Pow(normalizedSpeed, exponent);

        // Map curved value to target range
        float dynamicMaxHeight = Mathf.Lerp(targetMin, targetMax, curvedSpeed);

        for (int i = 0; i < bars.Length; i++)
        {
            float offset = (Mathf.Sin(Time.time * speeds[i]) * 0.5f + 0.5f) * dynamicMaxHeight;
            Vector3 pos = basePositions[i];
            pos.x += offset;
            bars[i].localPosition = pos;
        }
    }
}
