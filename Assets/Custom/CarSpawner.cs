using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public TrafficManager tm;
    private GameObject car;
    private CarAI carAI;
    public float spawnOffset;
    // Start is called before the first frame update
    void Start()
    {
        int carNum = 0;
        //car = Transform.Instantiate(tm.Carlist[carNum],transform.position+transform.forward*spawnOffset,transform.rotation,tm.transform);
        carAI = car.GetComponent<CarAI>();
        carAI.nf = transform.GetComponent<NodeFollower>();
        //carAI.speed = tm.CarSpeeds[carNum];
        //carAI.steering = tm.CarSteers[carNum];
        //carAI.stopDist = tm.CarStopDists[carNum];
    }
}
