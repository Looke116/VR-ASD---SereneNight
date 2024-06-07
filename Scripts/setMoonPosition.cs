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

    // Update is called once per frame
    DateTime time;
    void Update()
    {
        UpdateMoonPosition();
    }
    private void UpdateMoonPosition()
    {
        //moonIllumination = MoonCalc.GetMoonIllumination(date);
        //Debug.Log("Moon pos: " + moonPos.Altitude * Mathf.Rad2Deg + " " + (180 + moonPos.Azimuth * Mathf.Rad2Deg) + " " + date.ToString());
        //Debug.Log("Moon illumination: " + moonIllumination.Phase);
        time = xrTime.getTime();

        moonPos = MoonCalc.GetMoonPosition(xrTime.getTime(), latitude, longitude);
        transform.eulerAngles = (new Vector3((float)(moonPos.Altitude) * Mathf.Rad2Deg, 180 + (float)moonPos.Azimuth * Mathf.Rad2Deg, 0));
    }
}
