using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowShrinkTouchInteraction : ITouchInteraction {
    
    private bool big;

    void Start()
    {
        this.big = false;
    }

    public override void ObjectTouched()
    {
        if (this.big)
        {
            this.transform.localScale = this.transform.localScale * 0.5f;
            this.big = false;
        } else
        {
            this.transform.localScale = this.transform.localScale * 2;
            this.big = true;
        }
    }
}
