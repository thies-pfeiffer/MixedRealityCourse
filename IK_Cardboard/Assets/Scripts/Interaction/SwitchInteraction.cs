using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchInteraction : IInteractableObject {

    public GameObject toggleVisibleObject;

    // The transform to flip
    public Transform switchTransform;

    // The initial state of the switch
    public bool on = false;


    private Quaternion onRotation = Quaternion.Euler(-30, 0, 0);
    private Quaternion offRotation = Quaternion.Euler(30, 0, 0);

    // Use this for initialization
    void Start()
    {
        Switch();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void ObjectDefocused()
    {
        
    }

    public override void ObjectFocused()
    {
        
    }

    public override void ObjectSelected()
    {
        on = !on;
        Switch();
    }

    
    private void Switch()
    {
        if (on)
        {
            // Set On Rotation
            switchTransform.localRotation = onRotation;
        }
        else
        {
            // Set Off Rotation
            switchTransform.localRotation = offRotation;
        }

        // If there is an object, enable/disable it
        if (toggleVisibleObject != null)
        {
            toggleVisibleObject.SetActive(on);
        }
    }

}
