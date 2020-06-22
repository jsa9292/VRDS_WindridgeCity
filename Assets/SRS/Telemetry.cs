using UnityEngine;
using System;

[ExecuteInEditMode]
public class Telemetry : MonoBehaviour
{

    public string apiMode = "api";  //constant to identify the package
    public string game = "BI_DrivingSim";  //constant to identify the game
    public string vehicle = "BI Car";  //constant to identify the vehicle
    public string location = "Brain Institute";  //constant to identify the location
    uint apiVersion = 102;  //constant of the current version of the api

    //gets the vehicle body to send speed to SRS
    public Rigidbody vehicleBody;
    public unityDrivingLatency ud;
    public float pitch;
    //// Update is called once per frame
    //void Update()
    //{

    //    SimRacingStudio.SimRacingStudio_SendTelemetry(apiMode.ToCharArray()
    //                                                 , apiVersion
    //                                                 , game.ToCharArray()
    //                                                 , vehicle.ToCharArray()
    //                                                 , location.ToCharArray()
    //                                                 , Convert.ToSingle(vehicleBody.velocity.magnitude * 3.6)
    //                                                 , 7000
    //                                                 , 8000
    //                                                 , -1
    //                                                 , pitch
    //                                                 , ud.roll_new
    //                                                 , ud.yaw_new
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 0
    //                                                 , 2
    //                                                 , 2
    //                                                 , 2
    //                                                 , 2);
    //}
}