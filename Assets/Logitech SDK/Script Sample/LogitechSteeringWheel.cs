using UnityEngine;
using System.Collections;
using System.Text;

[ExecuteInEditMode]
public class LogitechSteeringWheel : MonoBehaviour {
	public UnityStandardAssets.Vehicles.Car.CarController carControl;
    public Vector3 Arrows;
    LogitechGSDK.LogiControllerPropertiesData properties;
    private string actualState;
    private string activeForces;
    private string propertiesEdit;
    private string buttonStatus;
    private string forcesLabel;
    string[] activeForceAndEffect;
	[Header ("Effects")]
	private int sideSwitch = 1;
	public int springCenter;
	public int springMagnitude;
	public int springFalloff;
	public int sideScale;
    public bool surfaceFX;
	public int surfaceMagMin;
	public int surfaceMagDelta;
	public int surfaceFreqMin;
	public int surfaceFreqDelta;
	[Header ("Inputs")]
	public float wheel;
	public float accel;
	public float brake;
	public bool reverse;
    public bool neutral;
    public bool park;
	public bool showGUI;
    public bool xButton;
    public bool yButton;
    public bool aButton;
    public bool bButton;
    public bool HomeButton;
    public float ignoreWheelUp;
    public float ignoreWheelDown;
	// Use this for initialization
	void Start () {
        //activeForces = "";
        //propertiesEdit = "";
        //actualState = "";
        //buttonStatus = "";
        //activeForceAndEffect = new string[9];
		Debug.Log(LogitechGSDK.LogiSteeringInitialize(false));
        

	}

//    void OnGUI()
//    {
//        activeForces = GUI.TextArea(new Rect(10, 10, 180, 200), activeForces, 400);
//        propertiesEdit = GUI.TextArea(new Rect(200, 10, 200, 200), propertiesEdit, 400);
//        actualState = GUI.TextArea(new Rect(410, 10, 300, 200), actualState, 1000);
//        buttonStatus = GUI.TextArea(new Rect(720, 10, 300, 200), buttonStatus, 1000);
//        GUI.Label(new Rect(10, 400, 800, 400), forcesLabel);
//    }
    public bool keyboard;
	// Update is called once per frame
	void Update () {
		if (keyboard) {
            if (Input.GetKey(KeyCode.C)) accel += 0.01f;
            else accel = 0f;
            if (Input.GetKey(KeyCode.X)) brake += .1f;
            else brake = 0;
            wheel = (Input.mousePosition.x/ 1535) -.5f;
            return;
        }

		//All the test functions are called on the first device plugged in(index = 0)
		if(LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0)){

            //CONTROLLER PROPERTIES
            StringBuilder deviceName = new StringBuilder(256);
            LogitechGSDK.LogiGetFriendlyProductName(0, deviceName, 256);
//			propertiesEdit = "Current Controller : "+ deviceName + "\n";
//            propertiesEdit += "Current controller properties : \n\n";
            //LogitechGSDK.LogiControllerPropertiesData actualProperties = new LogitechGSDK.LogiControllerPropertiesData();
            //LogitechGSDK.LogiGetCurrentControllerProperties(0, ref actualProperties);
//            propertiesEdit += "forceEnable = " + actualProperties.forceEnable + "\n";
//            propertiesEdit += "overallGain = " + actualProperties.overallGain + "\n";
//            propertiesEdit += "springGain = " + actualProperties.springGain + "\n";
//            propertiesEdit += "damperGain = " + actualProperties.damperGain + "\n";
//            propertiesEdit += "defaultSpringEnabled = " + actualProperties.defaultSpringEnabled + "\n";
//            propertiesEdit += "combinePedals = " + actualProperties.combinePedals + "\n";
//            propertiesEdit += "wheelRange = " + actualProperties.wheelRange + "\n";
//            propertiesEdit += "gameSettingsEnabled = " + actualProperties.gameSettingsEnabled + "\n";
//            propertiesEdit += "allowGameSettings = " + actualProperties.allowGameSettings + "\n";
            //CONTROLLER STATE
            actualState = "Steering wheel current state : \n\n";
            LogitechGSDK.DIJOYSTATE2ENGINES rec;
            rec = LogitechGSDK.LogiGetStateUnity(0);
//            actualState += "x-axis position :" + rec.lX + "\n";
//            actualState += "y-axis position :" + rec.lY + "\n";
//            actualState += "z-axis position :" + rec.lZ + "\n";
//            actualState += "x-axis rotation :" + rec.lRx + "\n";
//            actualState += "y-axis rotation :" + rec.lRy + "\n";
//            actualState += "z-axis rotation :" + rec.lRz + "\n";
//            actualState += "extra axes positions 1 :" + rec.rglSlider[0] + "\n";
//            actualState += "extra axes positions 2 :" + rec.rglSlider[1] + "\n";
//
			wheel = rec.lX/32767f;
            if (wheel < ignoreWheelDown) wheel -= ignoreWheelDown;
            else if (wheel > ignoreWheelUp) wheel -= ignoreWheelUp;
            else wheel = 0;

            float prev_accel = accel;
            if (accel >= prev_accel) accel = (-rec.lZ/32767f)+1;
			else accel = prev_accel-Time.deltaTime/10f;
            brake = (-rec.lRz/32767f +1);
			reverse = ((rec.rgbButtons[16] ==128)||(rec.rgbButtons[14] == 128)||( rec.rgbButtons[12] == 128));
            park = rec.rgbButtons[16] == 128;
            neutral = true;
            for (int i = 12; i <= 17; i++)
             {
                if (rec.rgbButtons[i] == 128) neutral = false;
             }
            //Debug.Log(rec.rgbButtons[15]);

            switch (rec.rgdwPOV[0])
            {
                case (0): Arrows.x += 1f; break; // Up
//                case (4500): actualState += "POV : UP-RIGHT\n"; break;
                case (9000): Arrows.y += 1f; ; break; // Right
//                case (13500): actualState += "POV : DOWN-RIGHT\n"; break;
                case (18000): Arrows.x -= 1f; break; // Down
//                case (22500): actualState += "POV : DOWN-LEFT\n"; break;
                case (27000): Arrows.y -= 1f; break; // Left
//                case (31500): actualState += "POV : UP-LEFT\n"; break;
                default: actualState += "POV : CENTER\n"; break;
            }

            //Button status :
            HomeButton = rec.rgbButtons[10] == 128;

            aButton = rec.rgbButtons[0] == 128;//a 0
            bButton = rec.rgbButtons[1] == 128;//b 1
            xButton = rec.rgbButtons[2] == 128;//x 2
            yButton = rec.rgbButtons[3] == 128;//y 3
            
//            buttonStatus = "Button pressed : \n\n";
//            for (int i = 0; i < 128; i++)
//            {
//                if (rec.rgbButtons[i] == 128)
//                {
//                    buttonStatus += "Button " + i + " pressed\n";
//                }
//
//            }
            
            /* THIS AXIS ARE NEVER REPORTED BY LOGITECH CONTROLLERS 
             * 
             * actualState += "x-axis velocity :" + rec.lVX + "\n";
             * actualState += "y-axis velocity :" + rec.lVY + "\n";
             * actualState += "z-axis velocity :" + rec.lVZ + "\n";
             * actualState += "x-axis angular velocity :" + rec.lVRx + "\n";
             * actualState += "y-axis angular velocity :" + rec.lVRy + "\n";
             * actualState += "z-axis angular velocity :" + rec.lVRz + "\n";
             * actualState += "extra axes velocities 1 :" + rec.rglVSlider[0] + "\n";
             * actualState += "extra axes velocities 2 :" + rec.rglVSlider[1] + "\n";
             * actualState += "x-axis acceleration :" + rec.lAX + "\n";
             * actualState += "y-axis acceleration :" + rec.lAY + "\n";
             * actualState += "z-axis acceleration :" + rec.lAZ + "\n";
             * actualState += "x-axis angular acceleration :" + rec.lARx + "\n";
             * actualState += "y-axis angular acceleration :" + rec.lARy + "\n";
             * actualState += "z-axis angular acceleration :" + rec.lARz + "\n";
             * actualState += "extra axes accelerations 1 :" + rec.rglASlider[0] + "\n";
             * actualState += "extra axes accelerations 2 :" + rec.rglASlider[1] + "\n";
             * actualState += "x-axis force :" + rec.lFX + "\n";
             * actualState += "y-axis force :" + rec.lFY + "\n";
             * actualState += "z-axis force :" + rec.lFZ + "\n";
             * actualState += "x-axis torque :" + rec.lFRx + "\n";
             * actualState += "y-axis torque :" + rec.lFRy + "\n";
             * actualState += "z-axis torque :" + rec.lFRz + "\n";
             * actualState += "extra axes forces 1 :" + rec.rglFSlider[0] + "\n";
             * actualState += "extra axes forces 2 :" + rec.rglFSlider[1] + "\n";
             */

            int shifterTipe = LogitechGSDK.LogiGetShifterMode(0);
            string shifterString = "";
            if (shifterTipe == 1) shifterString = "Gated";
            else if (shifterTipe == 0) shifterString = "Sequential";
            else  shifterString = "Unknown";
            actualState += "\nSHIFTER MODE:" + shifterString;

			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CONSTANT));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DAMPER));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_DIRT_ROAD));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_BUMPY_ROAD));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SLIPPERY_ROAD));
			//Debug.Log(LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_CAR_AIRBORNE));

			LogitechGSDK.LogiStopDamperForce(0);
			LogitechGSDK.LogiStopBumpyRoadEffect(0);
			LogitechGSDK.LogiStopDirtRoadEffect(0);
			LogitechGSDK.LogiStopSpringForce(0);
            LogitechGSDK.LogiPlaySpringForce(0, springCenter, springMagnitude, springFalloff);
            //LogitechGSDK.LogiStopSurfaceEffectEffect(0);
            if (surfaceFX)
            {
                var surfaceMag = surfaceMagMin + surfaceMagDelta * carControl.CurrentSpeed / carControl.MaxSpeed;
                var surfaceFreq = surfaceFreqMin + surfaceFreqDelta * carControl.CurrentSpeed / carControl.MaxSpeed;
                LogitechGSDK.LogiPlaySurfaceEffect(0, 0, (int)surfaceMag, (int)surfaceFreq);
            }
            LogitechGSDK.LogiPlayDamperForce(0,0);
//			sideSwitch *= -1; 
//			int sideMag = (int) (carControl.CurrentSpeed/sideScale);
//			LogitechGSDK.LogiPlaySideCollisionForce(0,sideSwitch*sideMag);
			//Debug.Log(sideSwitch);

		}
		else if(!LogitechGSDK.LogiIsConnected(0))
		{
			 actualState = "PLEASE PLUG IN A STEERING WHEEL OR A FORCE FEEDBACK CONTROLLER";
		}
		else{
			actualState = "THIS WINDOW NEEDS TO BE IN FOREGROUND IN ORDER FOR THE SDK TO WORK PROPERLY";
		}
	}

	void OnApplicationQuit()
	{
		Debug.Log("SteeringShutdown:" + LogitechGSDK.LogiSteeringShutdown());
	}


}
