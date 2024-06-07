using System;
using System.Collections.Generic;
using Normal.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

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
        dataCollector.Record($"Player pressed the {card} button");

        if (card.Equals("home"))
        {
            SceneManager.LoadScene("Starting Room");
        }
        else if (card.Equals("forest"))
        {
            SceneManager.LoadScene("Forest Multiplayer");
        }

        var options = new Realtime.InstantiateOptions
        {
            ownedByClient = true,
            preventOwnershipTakeover = true,
            useInstance = realtime
        };

        var cardDisplay = Realtime.Instantiate("Prefabs/Card Display", options);
        
        cardDisplay.transform.position =
            Camera.main.transform.position + Camera.main.transform.forward
                                           + new Vector3(0, 0.2f, 0);

        var image = cardDisplay.GetComponentInChildren<Image>();
        image.overrideSprite = Resources.Load<Sprite>($"Images/{card}");

        cardDisplay.GetComponent<LazyFollow>().target = Camera.main.transform;

        displays.Enqueue(cardDisplay);

        Invoke(nameof(DestroyDisplay), 5);
    }

    private void DestroyDisplay()
    {
        Realtime.Destroy(displays.Dequeue());
    }
}