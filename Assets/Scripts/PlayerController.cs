using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		StartCoroutine ("StartDelay");
		var sliderControls = GameObject.Find ("UICanvas").GetComponentsInChildren<Slider> ();
		foreach (var control in sliderControls) {
			if (control.name == "Accelerator") {
				uiAccelerator = control;
				uiAccelerator.value = 0;
			} else if (control.name == "Brake") {
				uiBrake = control;
				uiBrake.value = 0;
			} else if (control.name == "MaximumVelocityAttribute") {
				uiMaximumVelocityAttribute = control;
				uiMaximumVelocityAttribute.value = MaximumVelocityIndex;
			} else if (control.name == "AccelerationAttribute") {
				uiAccelerationAttribute = control;
				uiAccelerationAttribute.value = AccelerationIndex;
			} else if (control.name == "BrakingAttribute") {
				uiBrakingAttribute = control;
				uiBrakingAttribute.value = BrakingIndex;
			} else if (control.name == "HandlingAttribute") {
				uiHandlingAttribute = control;
				uiHandlingAttribute.value = HandlingIndex;
			}
		}
	}

	IEnumerator StartDelay ()
	{
		Time.timeScale = 0;
		float pauseTime = Time.realtimeSinceStartup + 3f;
		while (Time.realtimeSinceStartup < pauseTime)
			yield return 0;
		Time.timeScale = 1;
	}

	// Update is called once per frame
	void Update ()
	{
		
		if (!acceleratorActive && Input.GetAxis ("Accelerate") != 0) {
			acceleratorActive = true;
		}

		if (!brakeActive && Input.GetAxis ("Brake") != 0) {
			brakeActive = true;
		}

		if (Input.anyKey) {
			lastInputKeyboard = true;
		} else if (Input.GetKey (KeyCode.Joystick1Button7) || Input.GetKey (KeyCode.Joystick1Button6)) {
			lastInputKeyboard = false;
		}
	}

	float ConvertToRange (float val, float oldMinimum, float oldMaximum,
	                      float newMinimum, float newMaximum)
	{
		return (val - oldMinimum) / (oldMaximum - oldMinimum) *
		(newMaximum - newMinimum) + newMinimum;
	}

	float GetAxisInput (string axisName)
	{
		return lastInputKeyboard ? Input.GetAxis (axisName) : ConvertToRange (Input.GetAxis (axisName), -1, 1, 0, 1);
	}

	void FixedUpdate ()
	{
		UpdateAttributes ();

		var rb = GetComponent<Rigidbody2D> ();
		if (acceleratorActive) {
			var axis = GetAxisInput ("Accelerate");
			var accelerationForce = axis * GetAcceleration (AccelerationIndex);
			rb.AddForce (transform.up * accelerationForce);
			uiAccelerator.value = axis;
		}

		if (brakeActive) {
			var axis = GetAxisInput ("Brake");
			var brakingForce = axis * GetBraking (BrakingIndex);
			rb.AddForce (transform.up * -brakingForce);
			uiBrake.value = axis;
		}

		rb.angularVelocity = Input.GetAxis ("Horizontal") * -GetTorque (HandlingIndex);

		var forwardVelocity = Vector3.Dot (rb.velocity, transform.up);
		var slipVelocity = Vector3.Dot (rb.velocity, transform.right);
		var gripFactor = slipVelocity > gripThreshold && forwardVelocity > 3f ? 1 - slipGrip : 1 - stuckGrip;

		rb.velocity = transform.up * forwardVelocity +
		transform.right * slipVelocity * gripFactor;
	}

	private float GetGeometricFactor (float initialValue, int index)
	{
		return initialValue * Mathf.Pow (1.5f, index);
	}

	private float GetAcceleration (int index)
	{
		const float baseAcceleration = 5f;
		return GetGeometricFactor (baseAcceleration, index);
	}

	private float GetBraking (int index)
	{
		const float baseBraking = 2.5f;
		return GetGeometricFactor (baseBraking, index);
	}

	private float GetTorque (int index)
	{
		const float baseTorque = 50f;
		return GetGeometricFactor (baseTorque, index);
	}

	void UpdateAttribute (ref int attribute, KeyCode keyIncrease, KeyCode keyDecrease)
	{
		int delta = 0;
		if (Input.GetKeyDown (keyIncrease)) {
			delta = 1;
		} else if (Input.GetKeyDown (keyDecrease)) {
			delta = -1;
		}

		attribute = (int)Mathf.Clamp (attribute + delta, 0, 5);
	}

	void UpdateAttributes ()
	{
		UpdateAttribute (ref MaximumVelocityIndex, KeyCode.U, KeyCode.J);
		UpdateAttribute (ref AccelerationIndex, KeyCode.I, KeyCode.K);
		UpdateAttribute (ref BrakingIndex, KeyCode.O, KeyCode.L);
		UpdateAttribute (ref HandlingIndex, KeyCode.P, KeyCode.Semicolon);

		uiMaximumVelocityAttribute.value = MaximumVelocityIndex;
		uiAccelerationAttribute.value = AccelerationIndex;
		uiBrakingAttribute.value = BrakingIndex;
		uiHandlingAttribute.value = HandlingIndex;
	}

	private float stuckGrip = 0.5f;
	private float slipGrip = 0.1f;
	private float gripThreshold = 0.5f;

	private bool acceleratorActive = false;
	private bool brakeActive = false;

	private bool lastInputKeyboard = true;

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
}