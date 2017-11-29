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
    }
    float ConvertToRange(float val, float oldMinimum, float oldMaximum,
                         float newMinimum, float newMaximum)
    {
        return (val - oldMinimum) / (oldMaximum - oldMinimum) *
               (newMaximum - newMinimum) + newMinimum;
    }

    void FixedUpdate()
    {
        var rb = GetComponent<Rigidbody2D>();
        if (acceleratorActive)
        {
            var accelerationForce = ConvertToRange(Input.GetAxis("Accelerate"), -1, 1, 0, 1) * acceleration;
            rb.AddForce(transform.up * accelerationForce);
            uiAccelerator.value = ConvertToRange(Input.GetAxis("Accelerate"), -1, 1, 0, 1);
        }

        if (brakeActive)
        {
            var brakingForce = ConvertToRange(Input.GetAxis("Brake"), -1, 1, 0, 1) * braking;
            rb.AddForce(transform.up * -brakingForce);
            uiBrake.value = ConvertToRange(Input.GetAxis("Brake"), -1, 1, 0, 1);
        }

        rb.angularVelocity = Input.GetAxis("Horizontal") * -torque;

        var forwardVelocity = Vector3.Dot(rb.velocity, transform.up);
        var slipVelocity = Vector3.Dot(rb.velocity, transform.right);
        var gripFactor = slipVelocity > gripThreshold && forwardVelocity > 3f ? 1 - slipGrip : 1 - stuckGrip;

        rb.velocity = transform.up * forwardVelocity +
                      transform.right * slipVelocity * gripFactor;
    }

    private float acceleration = 15f;
    private float braking = 5f;
    private float torque = 100f;
    private float stuckGrip = 0.5f;
    private float slipGrip = 0.1f;
    private float gripThreshold = 0.5f;

    private bool acceleratorActive = false;
    private bool brakeActive = false;

    private Slider uiAccelerator;
    private Slider uiBrake;
}
