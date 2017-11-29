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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!acceleratorActive && Input.GetAxis("Accelerate") != 0)
        {
            acceleratorActive = true;
        }

        if (!brakeActive && Input.GetAxis("Brake") != 0)
        {
            brakeActive = true;
        }

        if (Input.anyKey)
        {
            lastInputKeyboard = true;
        }
        else if (Input.GetKey(KeyCode.Joystick1Button7) || Input.GetKey(KeyCode.Joystick1Button6))
        {
            lastInputKeyboard = false;
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
        return lastInputKeyboard ? Input.GetAxis(axisName) : ConvertToRange(Input.GetAxis(axisName), -1, 1, 0, 1);
    }

    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (acceleratorActive)
        {
            var axis = GetAxisInput("Accelerate");
            var accelerationForce = axis * GetAcceleration(0);
            rb.AddForce(transform.up * accelerationForce);
            uiAccelerator.value = axis;
        }

        if (brakeActive)
        {
            var axis = GetAxisInput("Brake");
            var brakingForce = axis * GetBraking(0);
            rb.AddForce(transform.up * -brakingForce);
            uiBrake.value = axis;
        }

        rb.angularVelocity = Input.GetAxis("Horizontal") * -GetTorque(0);

        var forwardVelocity = Vector3.Dot(rb.velocity, transform.up);
        var slipVelocity = Vector3.Dot(rb.velocity, transform.right);
        var gripFactor = slipVelocity > gripThreshold && forwardVelocity > 3f ? 1 - slipGrip : 1 - stuckGrip;

        rb.velocity = transform.up * forwardVelocity +
                      transform.right * slipVelocity * gripFactor;
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

    private float stuckGrip = 0.5f;
    private float slipGrip = 0.1f;
    private float gripThreshold = 0.5f;

    private bool acceleratorActive = false;
    private bool brakeActive = false;

    private bool lastInputKeyboard = true;

    private Slider uiAccelerator;
    private Slider uiBrake;
}