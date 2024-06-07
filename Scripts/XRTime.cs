using Normal.Realtime;
using SunCalcNet;
using SunCalcNet.Model;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using stardata;
public class XRTime : MonoBehaviour
{
    [SerializeField] private Light sunLight;
    [SerializeField] private Light moonLight;
    [SerializeField] private Material daySkybox;
    [SerializeField] private Material nightSkybox;
    [SerializeField] private Transform stars;
    [SerializeField] public Camera MainCamera;
    [SerializeField] public Camera MainCameraNonVR;
    [SerializeField] public float latitude = 34.0522f;
    [SerializeField] public float longitude = -118.2437f;
	[SerializeField] public int time = 00;
    [SerializeField] public int timeLimit = 6;
    
    XRDataSync model;
    SunPosition sunPos;
    MoonPosition moonPos;
    bool useCrtTime = true;
    
    private DateTime startTime;
    private float dayDurationInSeconds = 1200f; // 20 minutes for dawn to dusk

    private void Awake()
    {
        startTime = DateTime.Now;
    }
    void Start()
    {
        RenderSettings.skybox = daySkybox;
    }
    
    int secs = 0;

	public DateTime getTime()
	{
    	float elapsedSeconds = (float)(DateTime.Now - startTime).TotalSeconds;

    	if (elapsedSeconds > dayDurationInSeconds)
    	{
        	startTime = DateTime.Now;
       	 	elapsedSeconds = 0;
   	 	}

    	double hoursToAdd = (elapsedSeconds / dayDurationInSeconds) * 24;
    	DateTime simulatedTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time, 0, 0).AddHours(hoursToAdd);

    	if (simulatedTime.Hour >= timeLimit && simulatedTime.Hour < timeLimit + 1)
    	{
        	ExitGame();
    	}
		// Debug.Log(simulatedTime);
    	return simulatedTime;
	}	

    void ExitGame()
    {
        Debug.Log("Time Passed");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public float getLatitude()
    {
        return latitude;
    }
    
    public float getLongitude()
    {
        return longitude;
    }
}
