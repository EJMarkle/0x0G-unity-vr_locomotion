using System;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Bike controls class 
/// Gets VR input and sets throttle, brake, and steering data.
/// Implements IBikeInput so it can be used by any system needing input data.
/// </summary>
public class BikeControls : MonoBehaviour, IBikeInput
{
    [Header("VR Input")]
    public Transform headset;
    public XRKnob throttleKnob;
    public XRKnob steeringKnob;

    [Header("Tilt Settings")]
    public float tiltDeadzone = 5f;
    public float maxTilt = 30f;

    // Internal control state
    private float _throttleInput;
    private float _steering;
    private float _tilt;
    private bool _isBraking;

    // IBikeInput interface properties
    public float throttleInput => _throttleInput;
    public float steering => _steering;
    public float tilt => _tilt;
    public bool isBraking => _isBraking;

    void Update()
    {
        GetThrottle();
        GetTilt();
        GetSteering();
    }

    private void GetSteering()
    {
        _steering = (steeringKnob.value - 0.5f) * 2f;
        _steering = -_steering; // Flip direction if needed
    }

    private void GetThrottle()
    {
        _throttleInput = 1f - throttleKnob.value;
    }

    private void GetTilt()
    {
        float headRoll = headset.eulerAngles.z;

        // Convert from [0, 360] to [-180, 180]
        if (headRoll > 180f)
            headRoll -= 360f;

        // Deadzone
        if (Mathf.Abs(headRoll) < tiltDeadzone)
        {
            _tilt = 0f;
            return;
        }

        float sign = Mathf.Sign(headRoll);
        float adjustedRoll = Mathf.Abs(headRoll) - tiltDeadzone;
        float normalizedTilt = adjustedRoll / (maxTilt - tiltDeadzone);
        _tilt = Mathf.Clamp(normalizedTilt * sign, -1f, 1f);
        _tilt = -_tilt; // Flip if needed
    }

    public void StartBraking()
    {
        _isBraking = true;
    }

    public void StopBraking()
    {
        _isBraking = false;
    }
}
