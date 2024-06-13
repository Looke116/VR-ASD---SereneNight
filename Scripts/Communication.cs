using System;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Communication : MonoBehaviour
{
    public Realtime realtime;
    public DataCollector dataCollector;

    private Queue<GameObject> displays;

    private void Start()
    {
        displays = new Queue<GameObject>();
    }

    public void ButtonPressed(String card)
    {
        dataCollector.Record($"Player pressed the \"{card}\" button");

        if (card.Equals("Acasa") && !SceneManager.GetActiveScene().name.Equals("Starting Room"))
        {
            dataCollector.Record("Changing scene: Safe Space");
            SceneManager.LoadScene("Starting Room");
        }
        else if (card.Equals("Padure") && !SceneManager.GetActiveScene().name.Equals("Forest Multiplayer"))
        {
            dataCollector.Record("Changing scene: Forest");
            SceneManager.LoadScene("Forest Multiplayer");
        }
        // else if (card.Equals("Padure") && !SceneManager.GetActiveScene().name.Equals("Forest Multiplayer"))
        // {
        //     dataCollector.Record("Changing scene: Forest");
        //     SceneManager.LoadScene("LowQual Forest");
        //     realtime.Connect("SereneNight");
        // }

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };
        
        var pos = Camera.main.transform.position + Camera.main.transform.forward + new Vector3(0, 0.2f, 0);
        var rot = new Quaternion();
        var cardDisplay = Realtime.Instantiate("Prefabs/Card Display", pos, rot, options);

        var model = cardDisplay.GetComponent<CardDisplay>();
        model.SetSprite($"Images/{card}");

        displays.Enqueue(cardDisplay);

        Invoke(nameof(DestroyDisplay), 5);
    }

    private void DestroyDisplay()
    {
        Realtime.Destroy(displays.Dequeue());
    }
}