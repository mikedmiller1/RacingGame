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
    }

    float GetAccelerationConstant(int index)
    {
        var baseAcceleration = 0.5f; // m/s/s
        switch (index)
        {
        case 0:
            return baseAcceleration;
        case 1:
            return baseAcceleration * 1.5f;
        case 2:
            return baseAcceleration * 2.25f;
        case 3:
            return baseAcceleration * 3.375f;
        case 4:
            return baseAcceleration * 5.0625f;
        default:
            return baseAcceleration * 7.59375f;
        }
    }

    float GetBrakingMultiplier(int index)
    {
        var baseBraking = 1; // m/s/s
        switch (index)
        {
        case 0:
            return baseBraking;
        case 1:
            return baseBraking * 1.5f;
        case 2:
            return baseBraking * 2.25f;
        case 3:
            return baseBraking * 3.375f;
        case 4:
            return baseBraking * 5.0625f;
        default:
            return baseBraking * 7.59375f;
        }
    }

    float GetMaximumVelocity(int index)
    {
        var baseMaximumVelocity = 0.01f; // m/s
        switch (index)
        {
        case 0:
            return baseMaximumVelocity;
        case 1:
            return baseMaximumVelocity * 1.5f;
        case 2:
            return baseMaximumVelocity * 2.25f;
        case 3:
            return baseMaximumVelocity * 3.375f;
        case 4:
            return baseMaximumVelocity * 5.0625f;
        default:
            return baseMaximumVelocity * 7.59375f;
        }
    }

    float GetHandlingConstant(int index)
    {
        var baseHandling = 12f; // degrees / s
        switch (index)
        {
        case 0:
            return baseHandling;
        case 1:
            return baseHandling * 1.5f;
        case 2:
            return baseHandling * 2.25f;
        case 3:
            return baseHandling * 3.375f;
        case 4:
            return baseHandling * 5.0625f;
        default:
            return baseHandling * 7.59375f;
        }
    }

    void FixedUpdate()
    {
        var acceleratorInput = Input.GetAxis("Vertical");
        var steeringInput = Input.GetAxis("Horizontal");

        //acceleration = transform.up * GetAccelerationConstant(AccelerationIndex) * acceleratorInput;
        acceleration = transform.up * (acceleratorInput > 0 ? acceleratorInput : coast);

        if (!Mathf.Approximately(steeringInput, 0))
        {
            acceleration = Quaternion.Euler(0, 0, -Mathf.Sign(steeringInput) * GetHandlingConstant(HandlingIndex)) * acceleration;
        }

        //velocity = Vector3.MoveTowards(transform.position, transform.position + acceleration, Time.fixedDeltaTime) - transform.position;
        //velocity = acceleration.normalized * Time.fixedDeltaTime;
        if (acceleratorInput > 0)
        {
            coast = 1;
            velocity = Vector3.Lerp(transform.up, acceleration.normalized, GetAccelerationConstant(AccelerationIndex));
            velocity = Vector3.ClampMagnitude(velocity, GetMaximumVelocity(MaximumVelocityIndex));
        }
        else if (coast > 0)
        {
            var slowFactor = 0.01f;
            if (acceleratorInput < 0)
            {
                slowFactor *= GetBrakingMultiplier(BrakingIndex);
            }

            coast -= slowFactor * Time.fixedDeltaTime;
            coast = Mathf.Clamp01(coast);

            velocity = Vector3.Lerp(Vector3.zero, velocity, coast);
            if (velocity.magnitude < 1e-4f)
            {
                velocity = Vector3.zero;
                coast = 0;
            }
        }

        uiAccelerator.value = Mathf.Clamp01(acceleratorInput);
        uiBrake.value = Mathf.Abs(Mathf.Clamp(acceleratorInput, -1, 0));
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

    // Update is called once per frame
    void Update()
    {
        UpdateAttributes();

        if (Input.GetKeyDown(KeyCode.C))
        {
            transform.position = Vector3.zero;
        }

        Debug.DrawRay(transform.position, acceleration * 1000f, Color.cyan);
        Debug.DrawRay(transform.position, velocity * 100f, Color.magenta);
        Debug.DrawRay(transform.position, transform.up * 10f, Color.yellow);

        var degreesToRotate = 0f;
        if (velocity.magnitude > 0)
        {
            degreesToRotate = Vector3.SignedAngle(transform.up, velocity, Vector3.forward);
            if (Mathf.Abs(degreesToRotate) > 110f)
            {
                degreesToRotate = Vector3.SignedAngle(transform.up, -velocity, Vector3.forward);
            }
        }

        uiSteering.text = degreesToRotate.ToString();
        transform.Rotate(Vector3.forward, Mathf.Lerp(0, degreesToRotate, Time.deltaTime));

        uiSpeed.text = velocity.magnitude.ToString();
        transform.position += velocity;
    }

    private Text uiSpeed;
    private Text uiSteering;
    private Slider uiAccelerator;
    private Slider uiBrake;

    private Slider uiMaximumVelocityAttribute;
    private Slider uiAccelerationAttribute;
    private Slider uiBrakingAttribute;
    private Slider uiHandlingAttribute;

    private Vector3 acceleration;
    private Vector3 velocity;

    private float coast;

    [SerializeField]
    private int MaximumVelocityIndex;

    [SerializeField]
    private int AccelerationIndex;

    [SerializeField]
    private int BrakingIndex;

    [SerializeField]
    private int HandlingIndex;
}
