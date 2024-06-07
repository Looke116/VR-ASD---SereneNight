using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.UI;
using System;
using SunCalcNet.Model;
using SunCalcNet;
using UnityEngine.XR;

//based on https://normcore.io/documentation/realtime/synchronizing-custom-data.html#testing-it-out
//based on https://www.youtube.com/watch?v=J78uxCPO4rs&t=913s
public class XRDataSync : RealtimeComponent<XRPlayerDataModel>
{
    private void Start()
    {
        // Update the model initially and periodically.
        InvokeRepeating("UpdateModelWithCurrentTimeAndLocation", 0, 1.0f); // Update every second
    }

    private void UpdateModelWithCurrentTimeAndLocation()
    {
        // Ensure we're connected before trying to update the model.
        Realtime realtime = GameObject.FindGameObjectWithTag("NormcoreRealtime").GetComponent<Realtime>();
        if (realtime.connected)
        {
            DateTime now = DateTime.Now;
            // Update the model with current system time and predefined location.
            model.hour = now.Hour;
            model.minute = now.Minute; // Assuming your data model has a field for seconds.

            // Set latitude and longitude to predetermined values.
            model.latitude = 34.0522f;
            model.longitude = -118.2437f;
            
        }
    }
    
    protected override void OnRealtimeModelReplaced(XRPlayerDataModel previousModel, XRPlayerDataModel currentModel)
    {
        if (previousModel != null)
        {
            // Unsubscribe from previous model events.
            previousModel.hourDidChange -= HourChanged;
            previousModel.minuteDidChange -= MinuteChanged;
        }

        if (currentModel != null)
        {
            // Subscribe to the current model events to handle changes.
            currentModel.hourDidChange += HourChanged;
            currentModel.minuteDidChange += MinuteChanged;
        }
    }
    
    private void HourChanged(XRPlayerDataModel model, int value)
    {
        Debug.Log("Hour changed: " + value);
    }

    private void MinuteChanged(XRPlayerDataModel model, int value)
    {
        Debug.Log("Minute changed: " + value);
    }

}
