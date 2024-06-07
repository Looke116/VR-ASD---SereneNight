using System;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;

public class PersonalSpace : MonoBehaviour
{
    // public XRRayInteractor rayInteractor;
    public GameObject controller;
    [Tooltip("If you want to use A/B X/Y buttons on the controllers you might need to add the in your XRI Default Input Actions")]
    public InputActionProperty toggleOnOff;

    // private InputDevice _controller;
    private CapsuleCollider _personalSpace;
    private GameObject _personalSpaceDisplay;
    private bool _isEnabled;

    void Start()
    {
        // Get reference to the object's collider
        _personalSpace = GetComponent<CapsuleCollider>();

        // Disable collision between the object's layer and the default layer and the UI layer
        int currentLayer = _personalSpace.gameObject.layer;
        // Physics.IgnoreLayerCollision(currentLayer, 0);
        // Physics.IgnoreLayerCollision(currentLayer, 5);

        // Code to disable collision with all other layers if needed
        for (int i = 0; i < 32; i++)
        {
            if (i != currentLayer) Physics.IgnoreLayerCollision(currentLayer, i);
        }

        // Reference to the game object that is used to display the personal space
        _personalSpaceDisplay = gameObject.GetNamedChild("Display Mesh");

        // Whenever the toggle on/off action is performed call the "Toggle" local function 
        if (toggleOnOff.reference != null)
        {
            toggleOnOff.reference.action.performed += Toggle;
        }
        else
        {
            toggleOnOff.action.performed += Toggle;
        }

        return;
        
        // Declared local function 
        void Toggle(InputAction.CallbackContext obj)
        {
            _isEnabled = !_isEnabled;
        }

        // var leftHandedControllers = new List<UnityEngine.XR.InputDevice>();
        // var desiredCharacteristics = UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Left | UnityEngine.XR.InputDeviceCharacteristics.Controller;
        // UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(desiredCharacteristics, _controller);
        //
        // foreach (var device in leftHandedControllers)
        // {
        //     Debug.Log(string.Format("Device name '{0}' has characteristics '{1}'", device.name, device.characteristics.ToString()));
        // }
    }

    private void FixedUpdate()
    {
        if (!_isEnabled) return;
            
        // Get the player's position and normalize the height
        Vector3 playerPos = transform.position;
        playerPos.y = 0;

        // Cast a ray out of the controller to determine where it is pointing
        Physics.Raycast(controller.transform.position, controller.transform.forward, out var hit);
        
        // Check if the ray cast hit anything and save the result
        if (hit.collider == null) return;
        var hitPos = hit.point;
        hitPos.y = 0;

        // rayInteractor.TryGetHitInfo(out var hitPos, out var normal, out _, out _);
        // hitPos.y = 0;
        
        // Calculate the distance
        float distance = Vector3.Distance(playerPos, hitPos);

        // Clamp the size
        if (distance > 5)
        {
            distance = 5;
        }
        
        // Update the personal space
        _personalSpace.radius = distance;
        _personalSpaceDisplay.transform.localScale = new Vector3(distance * 2, 1, distance * 2);
    }
}