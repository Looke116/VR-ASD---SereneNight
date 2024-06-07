using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataCollector : MonoBehaviour
{
    public Transform characterCamera;
    public int interval = 10;

    private string _path;
    private StreamWriter _writer;
    private StringBuilder _stringBuilder;
    private static List<DataPoint> _dataPoints;

    private void Awake()
    {
        // Initialize variables
        _path = Application.persistentDataPath + "/player " + DateTime.Now.ToString("yyyy-M-d HH-mm") + ".json";
        // _path = Application.persistentDataPath + "/player.json";
        _writer = new StreamWriter(_path);
        _stringBuilder = new StringBuilder();

        if (_dataPoints == null)
        {
            _dataPoints = new List<DataPoint>();
        }
        
        // Call the function Record() repeatedly starting after 0 seconds and with the specified interval
        InvokeRepeating(nameof(Record), 0, interval);
    }

    // Track if the application looses focus
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            _dataPoints.Add(new DataPointMessage("Lost focus/quit"));
            CancelInvoke();
        }
        else
        {
            _dataPoints.Add(new DataPointMessage("Regained focus"));
            InvokeRepeating(nameof(Record), 0, interval);
        }
    }

    // Before the application quits format the data points into JSON format and save them to a file
    
    // If the app is ran on Quest/Android change the function to "OnApplicationPause" instead of "OnApplicationQuit"
    // private void OnApplicationPause(bool pauseStatus)
    // {
    //     if (!pauseStatus) return;
    private void OnApplicationQuit()
    {
        _stringBuilder.Append("[");

        for (int i = 0; i < _dataPoints.Count; i++)
        {
            _stringBuilder.Append(JsonUtility.ToJson(_dataPoints[i], true));
            _stringBuilder.Append(i != _dataPoints.Count - 1 ? "," : "]");
        }

        var jsonString = _stringBuilder.ToString();
        _writer.Write(jsonString);
        _writer.Close();
    }

    private void Record()
    {
        _dataPoints.Add(new DataPointTransform(
            characterCamera.position,
            characterCamera.rotation.eulerAngles,
            characterCamera.forward)
        );
    }

    // Public record function so it can be called from outside this script
    public void Record(String message)
    {
        // _dataPoints.Add(new DataPointMessage(message));
    }
}

internal class DataPoint
{
    private static int _id;
    public int id;
    public string dateTime;

    protected DataPoint()
    {
        id = _id;
        dateTime = DateTime.Now.ToString("u");

        _id++;
    }
}

internal class DataPointTransform : DataPoint
{
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Forward;

    public DataPointTransform(Vector3 position, Vector3 rotation, Vector3 forward)
    {
        Position = position;
        Rotation = rotation;
        Forward = forward;
    }
}

internal class DataPointMessage : DataPoint
{
    public string Message;

    public DataPointMessage(string message)
    {
        Message = message;
    }
}