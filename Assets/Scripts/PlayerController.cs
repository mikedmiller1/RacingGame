using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var sliderControls = GameObject.Find("UICanvas").GetComponentsInChildren<Slider>();
        foreach (var control in sliderControls)
        {
            if (control.name == "Accelerator")
            {
                uiAccelerator = control;
                uiAccelerator.value = 0;
            }
            else if (control.name == "Brake")
            {
                uiBrake = control;
                uiBrake.value = 0;
            }
            else if (control.name == "MaximumVelocityAttribute")
            {
                uiMaximumVelocityAttribute = control;
                uiMaximumVelocityAttribute.value = MaximumVelocityIndex;
            }
            else if (control.name == "AccelerationAttribute")
            {
                uiAccelerationAttribute = control;
                uiAccelerationAttribute.value = AccelerationIndex;
            }
            else if (control.name == "BrakingAttribute")
            {
                uiBrakingAttribute = control;
                uiBrakingAttribute.value = BrakingIndex;
            }
            else if (control.name == "HandlingAttribute")
            {
                uiHandlingAttribute = control;
                uiHandlingAttribute.value = HandlingIndex;
            }
        }

        // Get the reference to the sound controller
        Sound = SoundController.GetComponent<SoundController>();

        // Play the start-up sound
        Sound.PlayStartUpSound();

        // Assign the current position to the previous position
        PreviousPosition.x = transform.position.x;
        PreviousPosition.y = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (!joyAcceleratorActive && Input.GetAxis("AccelerateJoy") != 0)
        {
            joyAcceleratorActive = true;
        }

        if (!joyBrakeActive && Input.GetAxis("BrakeJoy") != 0)
        {
            joyBrakeActive = true;
        }

        if (useKeyboard)
        {
            if (joyAcceleratorActive && Input.GetAxis("AccelerateJoy") != -1f ||
                joyBrakeActive && Input.GetAxis("BrakeJoy") != -1f)
            {
                useKeyboard = false;
            }
        }
        else if (Input.GetAxis("Accelerate") != 0 || Input.GetAxis("Brake") != 0)
        {
            useKeyboard = true;
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            joyAcceleratorActive = false;
            joyBrakeActive = false;
        }
    }

    float ConvertToRange(float val, float oldMinimum, float oldMaximum,
                         float newMinimum, float newMaximum)
    {
        return (val - oldMinimum) / (oldMaximum - oldMinimum) *
               (newMaximum - newMinimum) + newMinimum;
    }

    float GetAxisInput(string axisName)
    {
        if (useKeyboard)
        {
            return Input.GetAxis(axisName);
        }
        else
        {
            var axis = -1f;
            if (axisName == "Accelerate")
            {
                if (joyAcceleratorActive)
                {
                    axis = Input.GetAxis("AccelerateJoy");
                }
            }
            else if (axisName == "Brake")
            {
                if (joyBrakeActive)
                {
                    axis = Input.GetAxis("BrakeJoy");
                }
            }

            return ConvertToRange(axis, -1, 1, 0, 1);
        }
    }

    void FixedUpdate()
    {
        UpdateAttributes();

        var rb = GetComponent<Rigidbody2D>();
        var axis = GetAxisInput("Accelerate");
        var accelerationForce = axis * GetAcceleration(AccelerationIndex);
        rb.AddForce(transform.up * accelerationForce);
        uiAccelerator.value = axis;

        axis = GetAxisInput("Brake");
        var brakingForce = axis * GetBraking(BrakingIndex);
        rb.AddForce(transform.up * -brakingForce);
        uiBrake.value = axis;

        var forwardVelocity = Vector3.Dot(rb.velocity, transform.up);
        var torqueFactor = Mathf.Lerp(0, GetTorque(HandlingIndex), Mathf.Abs(forwardVelocity));
        rb.angularVelocity = Input.GetAxis("Horizontal") * -torqueFactor;

        var slipVelocity = Vector3.Dot(rb.velocity, transform.right);
        var gripFactor = slipVelocity > gripThreshold && forwardVelocity > 3f ? 1 - slipGrip : 1 - stuckGrip;

        rb.velocity = transform.up * forwardVelocity +
                      transform.right * slipVelocity * gripFactor;


        // Calculate the acceleration
        Vector2 CurrentAccel = ((Vector2)transform.position - PreviousPosition) / Time.fixedDeltaTime;

        // Check the acceleration threshold
        if( accelerationForce > 0 && (CurrentAccel.x <= -AccelSoundThreshold) )
        {
            Sound.PlayAccelerationSound();
        }
        // Check the braking threshold
        else if( brakingForce > 0 && CurrentAccel.x >= BrakeSoundThreshold )
        {
            Sound.PlayBrakingSound();
        }
        // Check the idling threshold
        else if( Mathf.Abs(rb.velocity.x) <= IdleSoundThreshold )
        {
            Sound.PlayIdlingSound();
        }
        // Otherwise coasting
        else
        {
            Sound.PlayCoastingSound();
        }

        // Check the cornering acceleration
        if( CurrentAccel.y >= CorneringSoundThreshold )
        {
            Sound.PlayCorneringSound();
        }
        else
        {
            Sound.StopCorneringSound();
        }

        // Assign the current position to the previous position
        PreviousPosition.x = transform.position.x;
        PreviousPosition.y = transform.position.y;
    }

    private float GetGeometricFactor(float initialValue, int index)
    {
        return initialValue * Mathf.Pow(1.5f, index);
    }

    private float GetAcceleration(int index)
    {
        const float baseAcceleration = 5f;
        return GetGeometricFactor(baseAcceleration, index);
    }

    private float GetBraking(int index)
    {
        const float baseBraking = 2.5f;
        return GetGeometricFactor(baseBraking, index);
    }

    private float GetTorque(int index)
    {
        const float baseTorque = 50f;
        return GetGeometricFactor(baseTorque, index);
    }

    void UpdateAttribute(ref int attribute, KeyCode keyIncrease, KeyCode keyDecrease)
    {
        int delta = 0;
        if (Input.GetKeyDown(keyIncrease))
        {
            delta = 1;
        }
        else if (Input.GetKeyDown(keyDecrease))
        {
            delta = -1;
        }

        attribute = (int)Mathf.Clamp(attribute + delta, 0, 5);
    }

    void UpdateAttributes()
    {
        UpdateAttribute(ref MaximumVelocityIndex, KeyCode.U, KeyCode.J);
        UpdateAttribute(ref AccelerationIndex, KeyCode.I, KeyCode.K);
        UpdateAttribute(ref BrakingIndex, KeyCode.O, KeyCode.L);
        UpdateAttribute(ref HandlingIndex, KeyCode.P, KeyCode.Semicolon);

        uiMaximumVelocityAttribute.value = MaximumVelocityIndex;
        uiAccelerationAttribute.value = AccelerationIndex;
        uiBrakingAttribute.value = BrakingIndex;
        uiHandlingAttribute.value = HandlingIndex;
    }

    private float stuckGrip = 0.5f;
    private float slipGrip = 0.1f;
    private float gripThreshold = 0.5f;

    private bool joyAcceleratorActive = false;
    private bool joyBrakeActive = false;

    private bool useKeyboard = true;

    private int MaximumVelocityIndex;
    private int AccelerationIndex;
    private int BrakingIndex;
    private int HandlingIndex;

    private Slider uiAccelerator;
    private Slider uiBrake;

    private Slider uiMaximumVelocityAttribute;
    private Slider uiAccelerationAttribute;
    private Slider uiBrakingAttribute;
    private Slider uiHandlingAttribute;

    [SerializeField]
    private GameObject SoundController;
    private SoundController Sound;
    private Vector2 PreviousPosition;
    [SerializeField]
    private float AccelSoundThreshold = 0.1f;
    [SerializeField]
    private float BrakeSoundThreshold = 0.1f;
    [SerializeField]
    private float CorneringSoundThreshold = 0.3f;
    [SerializeField]
    private float IdleSoundThreshold = 10;
}
