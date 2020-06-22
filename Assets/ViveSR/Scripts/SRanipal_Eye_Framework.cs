//========= Copyright 2018, HTC Corporation. All rights reserved. ===========
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ViveSR
{
    namespace anipal
    {
        namespace Eye
        {
            public class SRanipal_Eye_Framework : MonoBehaviour
            {
                public enum FrameworkStatus { STOP, START, WORKING, ERROR, NOT_SUPPORT }
                /// <summary>
                /// The status of the anipal engine.
                /// </summary>
                public static FrameworkStatus Status { get; protected set; }
                /// <summary>
                /// Whether to enable anipal's Eye module.
                /// </summary>
                public bool EnableEye = true;

                private static SRanipal_Eye_Framework Mgr = null;
                public static SRanipal_Eye_Framework Instance
                {
                    get
                    {
                        if (Mgr == null)
                        {
                            Mgr = FindObjectOfType<SRanipal_Eye_Framework>();
                        }
                        if (Mgr == null)
                        {
                            Debug.LogError("SRanipal_Eye_Framework not found");
                        }
                        return Mgr;
                    }
                }

                void Start()
                {
                    StartFramework();
                }

                void OnDestroy()
                {
                    StopFramework();
                }

                public void StartFramework()
                {
                    if (!EnableEye) return;
                    if (Status == FrameworkStatus.WORKING) return;
                    if (!SRanipal_Eye.IsViveProEye())
                    {
                        Status = FrameworkStatus.NOT_SUPPORT;
                        return;
                    }

                    Status = FrameworkStatus.START;
                    Error result = SRanipal_API.Initial(SRanipal_Eye.ANIPAL_TYPE_EYE, IntPtr.Zero);
                    if (result == Error.WORK)
                    {
                        Debug.Log("[SRanipal] Initial Eye : " + result);
                        Status = FrameworkStatus.WORKING;
                    }
                    else
                    {
                        Debug.LogError("[SRanipal] Initial Eye : " + result);
                        Status = FrameworkStatus.ERROR;
                    }
                }

                public void StopFramework()
                {
                    if (SRanipal_Eye.IsViveProEye())
                    {
                        if (Status != FrameworkStatus.STOP)
                        {
                            Error result = SRanipal_API.Release(SRanipal_Eye.ANIPAL_TYPE_EYE);
                            if (result == Error.WORK) Debug.Log("[SRanipal] Release Eye : " + result);
                            else Debug.LogError("[SRanipal] Release Eye : " + result);
                        }
                        else
                        {
                            Debug.Log("[SRanipal] Stop Framework : module not on");
                        }
                    }
                    Status = FrameworkStatus.STOP;
                }
            }
        }
    }
}