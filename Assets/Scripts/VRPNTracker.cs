using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class VRPNTracker : MonoBehaviour {
    // the name of the VRPN tracker
    // standard default is Tracker0
    public string tracker = "Tracker0";

    // the address of the server
    // leave as localhost if running on same computer
    public string server = "localhost";

    // the sensor number to track
    public int sensor = 0;

    // Size of the buffer for this sensor
    public int numRepetitions = 1;

	// Use this for initialization
	void Start () {
        vrpnInit();

	}
	
	// Update is called once per frame
	void Update () {
        if (vrpnTrackerHasData(handle, sensor))
        {
            // To Do: check for overflow 
            IntPtr tdPointer = vrpnTrackerGetData(handle, sensor);
            if(tdPointer!= null)
            {
                TrackerData trackerData = (TrackerData)Marshal.PtrToStructure(tdPointer, typeof(TrackerData));
                // check for errors
                    Quaternion rotation = Quaternion.identity;
				    // convert  VRPN rotation into Unity coordinate system
				    rotation = transform.rotation;
				    rotation.x = -(float)trackerData.rotation [0];
				    rotation.y = -(float)trackerData.rotation [1];
				    rotation.z = (float)trackerData.rotation [2];
				    rotation.w = (float)trackerData.rotation [3];
                    transform.rotation = rotation;

                    // convert VRPN position into Unity coordinate system
			        Vector3 position = transform.position;
			        position.x = (float)trackerData.position [0];
			        position.y = (float)trackerData.position [1];
			        position.z = -(float)trackerData.position [2];

				    transform.position = position;
    
            }
        }
	}

    void OnApplicationQuit()
    {
        vrpnDestroy();
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TrackerData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public double[] position;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public double[] rotation;
    }

    private int handle;

    [DllImport("vrpnUnity")]
    static extern void vrpnInit();
    [DllImport("vrpnUnity")]
    static extern void vrpnDestroy();
    [DllImport("vrpnUnity")]
    static extern int vrpnInitializeTracker(string serverName, int sensorNumber, int numRepetitions);

    [DllImport("vrpnUnity")]
    static extern  bool vrpnTrackerHasData(int handle, int sensorNumber);
	[DllImport("vrpnUnity")]
    static extern IntPtr vrpnTrackerGetData(int handle, int sensorNumber);

    [DllImport("vrpnUnity")]
    static extern void getLastErrorAndClear(string str, int n);

}
