using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panorama : MonoBehaviour {

    // The Teleport destination may have a name
    public string name;

    // All connected panoramas 
    public Panorama[] connectedPanoramas;

    // The Prefab for teleporting
    public TeleportDestination teleportDestinationPrefab;


	// Use this for initialization
	void Start () {

        foreach (Panorama pano in connectedPanoramas)
        {
            // Instantiate TeleportDestination
            TeleportDestination teleport = Instantiate(teleportDestinationPrefab);
            teleport.transform.parent = this.transform;
            // Set Position and orientation
            teleport.transform.position = this.transform.position + 0.49f * this.transform.localScale.x * (pano.transform.position - this.transform.position).normalized;
            teleport.transform.LookAt(this.transform);
            teleport.transform.Rotate(new Vector3(90, 0, 0));
            // Set target transform
            teleport.proxyTransform = pano.transform;
            // Set text
            teleport.SetText(pano.name);
        }

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
