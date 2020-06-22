using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
public class unityDrivingLatency : MonoBehaviour {

	public float targetFrameRate = 90;
	public string NextScene;
	public UnityStandardAssets.Vehicles.Car.CarController carControl;
	public LogitechSteeringWheel Logitech;
	public Rigidbody Car;
	public Transform driverPos;
	public Text speedDisplay;
	public GameObject steerWheel;
	public GameObject Speedometer;
	public GameObject Tachometer;


	[Header("Simulator Inputs")]
	public float applyScale;
	public float brakeScale;
	public float pitch_const;
	public float roll_const;
	public float yaw_const;
	public float washOutYaw;
	public float Bumpyness;
	public float bodyRollConst, bodyPitchConst;
	public float defaultPitch, defaultRoll;

	[Header("Vehicle Configuration")]
	public float defaultAccel;
	private float reverse;
	private float neutral;
	public float acceleration;
	public float brake;

	[Header ("Current Vehicle State")]
	public float CurrentSpeed_z;
	public float CurrentSpeed_x;
	public float CurrentAccel;
	public float CurrentAccel_z;
	public float CurrentAccel_x;
	public float angularVelocity;
	public float angularAccel;
	public float turnRadius;

	[Header ("Previous Vehicle State")]
	private float previousAccel_Input;
	private Vector3 prevForward;
	private float previousSpeed_z;
	private float previousSpeed_x;
	private float previousAccel_z;
	private float previousAccel_x;
	private float previousAngularV;
	private float previousAngularA;

	[Header("Filter Parameters")]
	public float lp_weight;
	public float hp_weight;
	public float lp_lim;
	[Header("Filtered")]
	public float pitch_hp;
	public float pitch_lp;
	public float roll_hp;
	public float roll_lp;
	public float yaw_hp;



	public float bodypitch, bodyroll;
	public float pitch, roll, yaw;

	//Frame Control
	public int storedFrameCount;
	public List<float> pitchs = new List<float>();
	public List<float> rolls = new List<float>();
	public List<float> yaws = new List<float>();
	//System Ticks
	private long startTick;
	private List<long> renderTicks = new List<long>();
	private List<long> simTicks = new List<long>();


	void Awake(){
		Physics.autoSimulation = false;
		QualitySettings.vSyncCount = 1;
		QualitySettings.maxQueuedFrames = 0;
		Application.targetFrameRate = (int) targetFrameRate;
		startTick = DateTime.Now.Ticks;
		foreach (WheelCollider w in carControl.m_WheelColliders) {
			w.ConfigureVehicleSubsteps(10, 10, 10);
		}
	}
	void Update()
	{
		//process input
		Logitech.springMagnitude = 60 + (int)CurrentSpeed_z * 10;
		reverse = Logitech.reverse ? -1f : 1f;
		neutral = Logitech.neutral ? 0f : 1f;
		acceleration = (defaultAccel + (Logitech.accel+ accelTest)*0.9f) * neutral;
		brake = Logitech.brake * Mathf.Clamp(CurrentSpeed_z, 1f, 10f) / 10f;

		//Simulate Physics
		carControl.Move(wheelTest + Logitech.wheel,  acceleration, brakeTest + brake, 0, reverse);
		//Car.drag = 0.2f - carControl.m_GearNum*.05f;
		//Debug.Log(Time.smoothDeltaTime);
		float timeStep = 1f / (float)targetFrameRate;
		Physics.Simulate(timeStep);
		pitch = 0;
		roll = 0;
		yaw = 0;

		//Get the result of the simulation and generate the stats
		CurrentSpeed_z = Car.GetPointVelocity(transform.TransformPoint(driverPos.localPosition)).magnitude;
		//CurrentAccel_z = CurrentSpeed_z - previousSpeed_z;
		CurrentAccel = CurrentSpeed_z - previousSpeed_z;
		CurrentAccel_z = CurrentAccel * Mathf.Cos(angularVelocity); //forward
		CurrentAccel_x = CurrentAccel * Mathf.Sin(angularVelocity); //right
		angularVelocity = Vector3.SignedAngle(transform.forward, prevForward, transform.up);
		angularVelocity *= Mathf.PI / 180f;//Converting to radians
		angularAccel = angularVelocity - previousAngularV;
		bodyroll = (90f - Vector3.Angle(Vector3.up, Car.transform.right)) * bodyRollConst;
		bodypitch = (90f - Vector3.Angle(Vector3.up, Car.transform.forward)) * bodyPitchConst;
		//turnRadius = (CurrentSpeed_z / 2f);// Mathf.Cos(Mathf.PI / 2 - Mathf.Abs(angularVelocity));
		//Translation
		//z+ = surge
		//x+ = sway
		//y+ = heave
		//Rotation
		//x+ = pitch
		//y+ = yaw
		//z+ = roll
		//Inertial Frame of Reference
		pitch += CurrentAccel_z;
		roll += CurrentAccel_x;
		yaw += angularVelocity;
		//Rotational Frame of Reference
		//pitch += -Mathf.Abs(angularAccel) * turnRadius; //Euler
		//pitch += 2f* angularVelocity * CurrentSpeed; //Coriolis
		roll += angularVelocity * CurrentSpeed_z;//Mathf.Sign(angularVelocity) * Mathf.Pow(angularVelocity, 2) * CurrentSpeed;// turnRadius; //Centrifugal

		//Brake Scaling
		float brakePitchReduce = pitch < 0 ? brakeScale : 1f;//Reducing strong pitch forward caused by brakes 
		pitch *= brakePitchReduce;

		//Output Frame Control;
		if (pitchs.Count > storedFrameCount) pitchs.RemoveAt(0);
		pitchs.Add(pitch);
		if (rolls.Count > storedFrameCount) rolls.RemoveAt(0);
		rolls.Add(roll);
		if (yaws.Count > storedFrameCount) yaws.RemoveAt(0);
		yaws.Add(yaw);

		//pitch_result = pitchs[3];
		//roll_result = rolls[3];
		//yaw_result = yaws[3];
		int n = 3;
		//LowPassFilters
		float lp_coeff = lp_weight; // timeStep / (lp_weight + timeStep);
		pitch_lp = pitch_lp + Mathf.Clamp(lp_weight * (pitchs[n] - pitch_lp), -lp_lim, lp_lim);
		roll_lp = roll_lp + Mathf.Clamp(lp_weight * (rolls[n] - roll_lp), -lp_lim, lp_lim);
		//HighPassFilters
		float hp_coeff = hp_weight;//hp_weight / (hp_weight + timeStep);
		pitch_hp = hp_coeff * (pitch_hp + pitchs[n] - pitchs[n - 1]);
		roll_hp = hp_coeff * (roll_hp + rolls[n] - rolls[n - 1]);
		yaw_hp = hp_coeff * (yaw_hp + yaws[n] - yaws[n - 1]);
		//Combine LP and HP
		pitch_result = pitch_hp + pitch_lp;
		roll_result = roll_hp + roll_lp;
		yaw_result = yaw_hp;

		//adding vehicle body rotation, default rotations, and vibrations
		pitch_result += bodypitch + defaultPitch + CurrentSpeed_z / carControl.MaxSpeed * UnityEngine.Random.Range(-1f, 1f) * Bumpyness;
		roll_result += bodyroll + defaultRoll + CurrentSpeed_z / carControl.MaxSpeed * UnityEngine.Random.Range(-1f, 1f) * Bumpyness;

		//Clamp the values for the telemetry
		//Scale
		//Constant to scale for each axis
		pitch_result *= pitch_const;
		roll_result *= roll_const;
		yaw_result *= yaw_const;
		yaw_result -= Mathf.Sign(yaw_result) * washOutYaw; //Yaw Wash out Filter

		pitch_result = Mathf.Clamp(pitch_result, -20f, 20f);
		roll_result = Mathf.Clamp(roll_result, -20f, 20f);
		yaw_result = Mathf.Clamp(yaw_result, -3200f, 3200f);

		//		renderTicks.Add(DateTime.Now.Ticks - startTick);
	}
	void LateUpdate(){
		StartCoroutine(UpdateTelemetry());


		//Visual stuff
		//speedDisplay.text = ((int)(carControl.CurrentSpeed * 2.23693629f)).ToString();
		steerWheel.transform.localEulerAngles = new Vector3(-18, 0, Logitech.wheel * 450f);
		Speedometer.transform.localEulerAngles = new Vector3(-12, 0, -45f + carControl.CurrentSpeed * 2.37389f);// 2.23693629f * (260/245));
		Tachometer.transform.localEulerAngles = new Vector3(-12, 0, 40 + carControl.Revs * 880f / 9f);
		//Update previous
		previousSpeed_z = CurrentSpeed_z;
		previousSpeed_x = CurrentSpeed_x;
		previousAccel_z = CurrentAccel_z;
		previousAccel_x = CurrentAccel_x;
		previousAngularV = angularVelocity;
		previousAccel_Input = acceleration;
		prevForward = transform.forward;

	}

	[Header("Telemetry Package")]
	public string apiMode = "api";  //constant to identify the package
	public string game = "BI_DrivingSim";  //constant to identify the game
	public string vehicle = "BI Car";  //constant to identify the vehicle
	public string location = "Brain Institute";  //constant to identify the location
	uint apiVersion = 102;  //constant of the current version of the api

	[Header("Debug")]
	public float wheelTest;
	public float accelTest;
	public float brakeTest;
	public float pitchTest;
	public float rollTest;
	public float yawTest;
	public float pitch_result, roll_result, yaw_result;
	public IEnumerator UpdateTelemetry(){
		
			yield return new WaitForEndOfFrame();
			SimRacingStudio.SimRacingStudio_SendTelemetry(apiMode.ToCharArray()
															, apiVersion
															, game.ToCharArray()
															, vehicle.ToCharArray()
															, location.ToCharArray()
															, 0
															, 0
															, 0
															, 0
															, pitch_result*applyScale + pitchTest//pitch
															, roll_result * applyScale + rollTest//roll
															, yaw_result * applyScale + yawTest//yaw
															, 0//speed
															, 0//surge
															, 0//heave
															, 0//sway
															, 0
															, 0
															, 0
															, 0
															, 0
															, 0
															, 0
															, 0);
			
		yield return null;


	}
	//Second order washout filters
	float LowPassFilter (float s, float w, float z) {
		float gain = Mathf.Pow(w,2f) / (Mathf.Pow(s, 2f) + 2f * w * z + Mathf.Pow(w, 2f));
		return (s*gain);
		
	}
	float HighPassFilter (float s, float w, float z) {
		float gain = Mathf.Pow(s, 2f) / (Mathf.Pow(s, 2f) + 2 * w * z + Mathf.Pow(w, 2f));
		return (s * gain);
	}
}
