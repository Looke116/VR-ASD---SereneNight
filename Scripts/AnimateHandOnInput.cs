using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchInputAction;
    public InputActionProperty gripInputAction;
    
    public Animator handAnimator;
    
    void Update()
    {
        float triggerValue = pinchInputAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        
        float gripValue = gripInputAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);
    }
}
