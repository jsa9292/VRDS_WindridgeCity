using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public List<GameObject> cars;
    private GameObject car;
    private CarAI carAI;
    public Vector3 spawnOffset;
    // Start is called before the first frame update
	public void Spawn(Transform spawneeParent)
    {
        int carNum = 0;
        car = cars[Random.Range(0, cars.Count)];
		car = Transform.Instantiate(car,transform.position+transform.forward*spawnOffset.x +transform.up*spawnOffset.y +transform.right*spawnOffset.z,transform.rotation,spawneeParent);
        carAI = car.GetComponent<CarAI>();
        carAI.nf = transform.GetComponent<NodeFollower>();
        //carAI.speed = tm.CarSpeeds[carNum];
        //carAI.steering = tm.CarSteers[carNum];
        //carAI.stopDist = tm.CarStopDists[carNum];
    }
}
