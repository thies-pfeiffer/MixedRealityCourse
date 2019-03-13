using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDestination : MonoBehaviour {

    [Tooltip("Transform to set position to - If not set, this transformation is used")]
    public Transform proxyTransform;

    // Our player's transform
    private Transform player;
    // And save the initial size of the player
    private float playerSize;

    

    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player").transform;
        playerSize = player.transform.position.y;
        
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void StartedGazingAt()
    {
        Debug.Log("Start");
        // Start particle system
        try
        {
            GetComponentInChildren<ParticleSystem>().Play();
        } catch (Exception e)
        { 
            // Just in case no particle system is used
        }

    }

    public void StoppedGazingAt()
    {
        // Stop particle system
        try
        {
            GetComponentInChildren<ParticleSystem>().Stop();
        }
        catch (Exception e)
        {
            // Just in case no particle system is used
        }

    }

    public void Teleport()
    {
        Debug.Log("Teleport");
        if (proxyTransform != null)
        {
            player.transform.position = new Vector3(proxyTransform.position.x, playerSize + proxyTransform.position.y, proxyTransform.position.z);
        }
        else
        {
            player.transform.position = new Vector3(transform.position.x, playerSize + transform.position.y, transform.position.z);
        }
    }

    public void SetText(string text)
    {
        TextMesh textMesh = GetComponentInChildren<TextMesh>();
        if (textMesh != null)
        {
            textMesh.text = text;
        } else
        {
            Debug.LogWarning("No TextMesh attached at TeleportDestination!");
        }
    }
}

