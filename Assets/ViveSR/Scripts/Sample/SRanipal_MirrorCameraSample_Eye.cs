//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            /// <summary>
            /// A very basic mirror.
            /// </summary>
            [RequireComponent(typeof(Camera))]
            public class SRanipal_MirrorCameraSample_Eye : MonoBehaviour
            {
                private const float Distance = 0.6f;
				public Vector3 offset;
                private void Update()
                {
					SetMirroTransform();
                    if (SRanipal_Eye_Framework.Status != SRanipal_Eye_Framework.FrameworkStatus.WORKING) return;

                    //When gaze ray data is valid, place the mirror gameobject directly in front of the player camera.
                    Ray GazeRay;
                    if (SRanipal_Eye.GetGazeRay(GazeIndex.COMBINE, out GazeRay))
                    {
                        
                        enabled = false;
                    }
                }

                private void SetMirroTransform()
                {
                    //transform.position = Camera.main.transform.position + Camera.main.transform.forward * Distance;
                    //transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z);
                    transform.LookAt(Camera.main.transform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0) +offset;
				
                }
            }
        }
    }
}