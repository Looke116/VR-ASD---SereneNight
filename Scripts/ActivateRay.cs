using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateRay : MonoBehaviour
{
	public GameObject leftTeleporterRay;
	public GameObject rightTeleporterRay;
	
	public InputActionProperty leftTeleport;
	public InputActionProperty rightTeleport;

    void Update()
    {
        leftTeleporterRay.SetActive(leftTeleport.action.ReadValue<float>() > 0.1f);
		rightTeleporterRay.SetActive(rightTeleport.action.ReadValue<float>() > 0.1f);
    }
}
