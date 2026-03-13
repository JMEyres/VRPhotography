using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


public class TestScript : XRBaseInteractable
{
    public InputActionReference button; // assign this in the inspector to get a button from the controller

    protected override void OnEnable()
    {
        button.action.performed += TESTFunc;
    }

    protected override void OnDisable()
    {
        button.action.performed += TESTFunc;
    }
    
    void TESTFunc(InputAction.CallbackContext context)
    {
        Debug.Log("BUTTON PRESSED");
    }
}
