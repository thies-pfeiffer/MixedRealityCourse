using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportDestination : MonoBehaviour {

    [Tooltip("Transform to set position to - If not set, this transformation is used")]
    public Transform proxyTransform;

    [Tooltip("Teleport can be activated using dwell time or using click (0 means click)")]
    public float dwellTime = 0;

    // Our player's camera
    private Transform player;
    // And save the initial size of the player
    private float playerSize;


    // If dwell time is used, remember the last time gaze was started 
    private float lastGazedAt;


    // Use this for initialization
    void Start () {
        player = GameObject.FindWithTag("Player").transform;
        playerSize = player.transform.position.y;
        
    }
	
	// Update is called once per frame
	void Update () {

        // Update dwell time
		if (lastGazedAt > 0)
        {
            if (Time.time - lastGazedAt >= dwellTime)
            {
                lastGazedAt = 0;
                Teleport();
            }
        }
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

        // Remember the gazing time
        if (dwellTime > 0)
        {
            lastGazedAt = Time.time;
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

        // Reset dwell timer
        lastGazedAt = 0;
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
            player.transform.position = new Vector3(transform.position.x, playerSize + proxyTransform.position.y, transform.position.z);
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

