using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IInteractableObject : MonoBehaviour {

    public abstract void ObjectFocused();

    public abstract void ObjectDefocused();

    public abstract void ObjectSelected();

}
