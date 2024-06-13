using SunCalcNet;
using SunCalcNet.Model;
using System;
using UnityEngine;
using UnityEngine.UI;

public class setMoonPosition : MonoBehaviour
{
    [SerializeField] int hour = DateTime.Now.Hour;
    [SerializeField] int minute = DateTime.Now.Minute;
    [SerializeField] public GameObject timeGameObject;

    private XRTime xrTime;
    private float latitude;
    private float longitude;

    DateTime date;
    MoonPosition moonPos;
    //MoonIllumination moonIllumination;
    // Start is called before the first frame update
    void Start()
    {   
        xrTime = timeGameObject.GetComponent<XRTime>();
        latitude = xrTime.getLatitude();
        longitude = xrTime.getLongitude();
    }

    float crtLat, crtLon;
    DateTime crtTime, newTime;
    // Update is called once per frame
    DateTime time;
     public void LateUpdate()
    {
        newTime = xrTime.getTime();
        // only update when the lat/lon have changed
        if (crtLat != latitude || crtLon != longitude || (Math.Abs((crtTime - newTime).TotalSeconds) > 1))
        {
            crtTime = newTime;
            crtLat = latitude;
            crtLon = longitude;
            UpdateMoonPosition();
        }
    }
    private void UpdateMoonPosition()
    {
        //moonIllumination = MoonCalc.GetMoonIllumination(date);
        //Debug.Log("Moon pos: " + moonPos.Altitude * Mathf.Rad2Deg + " " + (180 + moonPos.Azimuth * Mathf.Rad2Deg) + " " + date.ToString());
        //Debug.Log("Moon illumination: " + moonIllumination.Phase);
        // time = xrTime.getTime();

        moonPos = MoonCalc.GetMoonPosition(xrTime.getTime(), latitude, longitude);
        transform.eulerAngles = (new Vector3((float)(moonPos.Altitude) * Mathf.Rad2Deg, 180 + (float)moonPos.Azimuth * Mathf.Rad2Deg, 0));
    }
}
