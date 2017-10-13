using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestCarControl : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var textControls = GameObject.Find("UICanvas").GetComponentsInChildren<Text>();
        foreach (var control in textControls)
        {
            if (control.name == "Speed")
            {
                uiSpeed = control;
                uiSpeed.text = "0";
            }
            else if (control.name == "Steering")
            {
                uiSteering = control;
                uiSteering.text = "0";
            }
        }

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

    float GetAccelerationConstant(int index)
    {
        var baseAcceleration = 0.005f;
        switch (index) {
            case 0: return baseAcceleration;
            case 1: return baseAcceleration * 1.5f;
            case 2: return baseAcceleration * 2.25f;
            case 3: return baseAcceleration * 3.375f;
            case 4: return baseAcceleration * 5.0625f;
            default: return baseAcceleration * 7.59375f;
        }
    }

    float GetMaximumVelocity(int index)
    {
        var baseMaximumVelocity = 100f; // m/s
        switch (index) {
            case 0: return baseMaximumVelocity;
            case 1: return baseMaximumVelocity * 1.5f;
            case 2: return baseMaximumVelocity * 2.25f;
            case 3: return baseMaximumVelocity * 3.375f;
            case 4: return baseMaximumVelocity * 5.0625f;
            default: return baseMaximumVelocity * 7.59375f;
        }
    }

    void FixedUpdate()
    {
        var acceleratorInput = Input.GetAxis("Vertical");
        var steeringInput = Input.GetAxis("Horizontal");

        acceleration = transform.up;

//        if (acceleratorInput)
//        {
            acceleration *= GetAccelerationConstant(AccelerationIndex) * acceleratorInput;
//        }

        if (!Mathf.Approximately(steeringInput, 0))
        {
            acceleration += Quaternion.Euler(0, 0, -Mathf.Sign(steeringInput) * 12f) * acceleration;

//            acceleration = Quaternion.Euler(0, 0, Mathf.Sign(steeringInput) * 45f) * acceleration;
        }

        uiAccelerator.value = Mathf.Clamp01(acceleratorInput);
        uiBrake.value = Mathf.Abs(Mathf.Clamp(acceleratorInput, -1, 0));
        uiSteering.text = steeringInput.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, acceleration, Color.cyan);
        //uiSpeed.text = Time.time.ToString();
        transform.position += Vector3.Project(acceleration, transform.up);
    }

    private Text uiSpeed;
    private Text uiSteering;
    private Slider uiAccelerator;
    private Slider uiBrake;

    private Vector3 acceleration;

    [SerializeField]
    private int MaximumVelocityIndex;

    [SerializeField]
    private int AccelerationIndex;

    [SerializeField]
    private int BrakingIndex;

    [SerializeField]
    private int HandlingIndex;
}
