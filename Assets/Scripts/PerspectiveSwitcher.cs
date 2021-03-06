﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

// Written by James Bentley 19th May 2017
// Last updated 23rd of May 2017
// The intention of this class is to be attached to a camera which allows 
// the camera to switch it's position between various objects in the scene
// All the coroutines make it look scary but it's actually pretty simple!

// Basically This class subscribes to events called by VRInteractiveObjects in the scene. 
// The PlayerMount script sits on a prefab in my scene which includes a VRInteractiveItem as well as the PlayerMount script whcih hold several fields that are useful. 
public class PerspectiveSwitcher : MonoBehaviour {

    protected PlayerMount[] mounts;   // A list of all the PlayerMount objects in the scene. PlayerMounts are what we can sit atop.
    public float transferSpeed;     // The speed at which you teleport to the location.
    public float loadSpeed;         // The time you need to focus on any mount before your perspective is changed to it.
    public Image reticle;           // A Reference to the targetting Reticle Image that we need to fill up.
    protected Coroutine lastCoroutine; // A Reference to the last coroutine we called (the load coroutine) so we can kill it if we need to
    public delegate void MountAction();
    public static event MountAction OnMount;
    public AudioSource audio;

    public delegate void SwitchAction();
    public static event SwitchAction OnStartSwitch;

    // Use this for initialization
    protected void Start () {
        // Here we find all the mPlayerMount objects in the scene and add our callbacks to their events.
        mounts = FindObjectsOfType<PlayerMount>();
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver += startLoad;
            mounts[i].interactiveItem.OnOut += stopLoad;
        }

        if (!audio)
            audio = FindObjectOfType<AudioSource>();
    }

    // This is called when we disbale our object
    protected void OnDisable()
    {
        //Make sure we remove the callbacks so we don't get a memory leak.
        for (int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver -= startLoad;
            mounts[i].interactiveItem.OnOut -= stopLoad;
        }
    }

    // Update is called once per frame
    protected void Update () {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10);
    }

    // This method stops the "Load" method which is called when the OnOut event.
    public void stopLoad()
    {
        StopCoroutine(lastCoroutine);
        reticle.fillAmount = 0;
    }

    // Starts the coroutine that starts a timer which counts down until we transition to the new mount position
    public void startLoad()
    {
        PlayerMount targettedMount = getTargettedMount();
        if(targettedMount)
        {
            if (!targettedMount.isMounted)
            {
                lastCoroutine = StartCoroutine(Load(targettedMount));
            }
        }
    }

    // This is the coroutine which Loads up the reticle. Once the while loop is finished we switch our perspective to the new mount
    protected IEnumerator Load(PlayerMount targettedMount)
    {
        while(reticle.fillAmount < 1.0)
        {
            reticle.fillAmount += loadSpeed;
            yield return null;
        }
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].isMounted = false;
        }
        startSwitch(targettedMount);
    }

    // This kicks off the coroutine which changes the perspective
    public void startSwitch(PlayerMount targettedMount)
    {
        stopLoad();
        OnStartSwitch();
        audio.Play();
        lastCoroutine = StartCoroutine(switchPerspective(targettedMount));
    }

    // Here's the coroutine which moves the player to the new perspective
    virtual protected IEnumerator switchPerspective(PlayerMount targettedMount)
    {
        OnMount();

        yield return null;
    }

    // This is a utility function that gets the current targetted mount using it/s "IsOver" field.
    protected PlayerMount getTargettedMount()
    {
        for (int i = 0; i < mounts.Length; i++)
        {
            if (mounts[i].interactiveItem.IsOver)
            {
                return mounts[i];
            }
        }
        return null;
    }
}
