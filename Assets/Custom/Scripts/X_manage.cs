using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class X_manage : MonoBehaviour
{
    public bool detect; //pickup Node from children
    public float detectDist;
    public bool detectLights; //pickup MaterialChanger from LightGroupParents 
	public int numCars;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Node n in intersection) {
            n.isIntersectionNode = true;
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, detectDist);
        if (detect) {
            Collider[] c;
            enters.Clear();
            intersection.Clear();
            foreach (Node n in transform.GetComponentsInChildren<Node>())
            {
                intersection.Add(n);
                n.EqHeight = true;
            }

            int lm = 1 << 9;
            c = Physics.OverlapSphere(transform.GetChild(transform.childCount - 1).position, detectDist,lm);
            foreach (Collider i in c)
            {
                Node n;
                bool cond1 = i.transform.parent.name[0] != 'X';
                bool cond2 = i.transform.GetSiblingIndex() != 0;
                if ((n = i.transform.parent.GetComponent<Node>()) && cond2 && cond1) enters.Add(n);
            }
            foreach (Node n in enters) {
                n.detectNodes = true;
            }
            foreach (Node n in intersection) {
                n.detectNodes = true;
            }
            detect = false;
        }
        if (detectLights)
        {
            LightGroup1 = LightGroupParents[0].GetComponentsInChildren<MaterialChangerJP>();
            LightGroup2 = LightGroupParents[1].GetComponentsInChildren<MaterialChangerJP>();
            LightGroup3 = LightGroupParents[2].GetComponentsInChildren<MaterialChangerJP>();
            //LightGroup4 = LightGroupParents[3].GetComponentsInChildren<MaterialChanger>();
        }
    }


    public List<Node> enters;
    public List<Node> intersection;
    private int xI;
    private int prev_state;
    public List<bool> stateGroup;//pairs enters into binary groups
    public int state;
    //only supports binary states with wait inbetween
    public bool trafficLight; //boolean to control light groups
    public float signalDur;
    public float pauseDur;
    public List<GameObject> LightGroupParents;//gameobject parents for lights
    public MaterialChangerJP[] LightGroup1;
    public MaterialChangerJP[] LightGroup2;
    public MaterialChangerJP[] LightGroup3; //this is yellow light group

	private float TimeforSim;
    void Update()
    {
		numCars = 0;
		//for each intersection, if any of the conflict is occupied, it is disabled. 
		foreach (Node n in intersection) {
			numCars+=n.occupied;
        	foreach (Node m in n.conflicts) {
				m.stop = n.occupied> 0;
			}
		}

        //if(Time.realtimeSinceStartup >)
        if (state != prev_state)
        {
            foreach (MaterialChangerJP m in LightGroup1)
            {
                m.Switch(false);
            }
            foreach (MaterialChangerJP m in LightGroup2)
            {
                m.Switch(false);
            }
            foreach (MaterialChangerJP m in LightGroup3)
            {
                m.Switch(false);
            }
            switch (state)
            {
                /* Vertical or horizontal */
                case 1:
                    for (int i = 0; i < enters.Count; i++)
                    {
                        enters[i].exitOn = stateGroup[i];
                    }
                    if (trafficLight)
                    {
                        foreach (MaterialChangerJP m in LightGroup1)
                        {
                            m.Switch(true);
                        }
                    }
                    prev_state = 1;
                    break;
                /* Vertical or horizontal */
                case 2:
                    for (int i = 0; i < enters.Count; i++)
                    {
                        enters[i].exitOn = !stateGroup[i];
                    }
                    if (trafficLight)
                    {
                        foreach (MaterialChangerJP m in LightGroup2)
                        {
                            m.Switch(true);
                        }
					}
                    prev_state = 2;
                    break;
                /* all stop*/
				case 3:
					for (int i = 0; i < enters.Count; i++)
					{
						enters[i].exitOn = false;
					}
                    if (trafficLight)
                    {
                        foreach (MaterialChangerJP m in LightGroup3)
                        {
                            m.Switch(true);
                        }
                    }
                    prev_state = 3;
                    return;
                /* one car in the intersection at a time */
                default:
					enters[xI].exitOn = numCars == 0;
			        xI++;
			        if (xI >= enters.Count) xI = 0;
					prev_state = 3;
					return;
            }
        }
        // This enforces nodefollowers to waitfor execution of rest of X_manage script before allowed to switch to intersection node
        // The problem was 1, nodefollowers switched node before checking if it was available,
        // then the multiple instances of nodefollowers switched to intersection node before conflicts were updated.
        // instead of updating conflict for each switch which increases operations count,
        // shutting down allows reducing the operation count
//        enters[xI].exitOn = t;
//        xI++;
//        if (xI >= enters.Count) xI = 0;

		if (!trafficLight) return;
		TimeforSim += 1f / (float)Application.targetFrameRate;
		if (TimeforSim > 30f) TimeforSim = 0;
		if (TimeforSim > 0f) state = 1;
		if (TimeforSim > 10f) state = 3;
		if (TimeforSim > 15f) state = 2;
		if (TimeforSim > 25f) state = 3;
	}
}
