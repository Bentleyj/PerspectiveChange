using System.Collections;
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
// The PlayerMount script sits on a prefab in my scene which includes a VRInteractiveItem 
// as well as the PlayerMount script which hold several fields that are useful. 
public class PerspectiveSwitcherBlink : PerspectiveSwitcher {

    public Material mat;
    public new static event MountAction OnMount;
    public float blinkAmount;

    // Use this for initialization
    new void Start () {
        // Here we find all the mPlayerMount objects in the scene and add our callbacks to their events.
        base.Start();
        blinkAmount = 0;
    }

    new void Update()
    {
        base.Update();
        mat.SetFloat("_BlinkAmount", blinkAmount);
    }

    // Here's the coroutine which moves the player to the new perspective
    new private IEnumerator switchPerspective(PlayerMount targettedMount)
    {
        Transform target = targettedMount.transform;
        //while (blinkAmount < 1.0)
        //{
        //    blinkAmount += transferSpeed/2;
        //    yield return null;
        //}
        targettedMount.isMounted = true;
        transform.position = target.position;
        //for (int i = 0; i < targettedMount.mountableAvatarParts.Length; i++)
        //{
        //    targettedMount.mountableAvatarParts[i].SetActive(false);
        //}

        ////yield return null;
        //while (blinkAmount > 0.0)
        //{
        //    blinkAmount -= transferSpeed / 2;
        //    yield return null;
        //}



        //yield return null;
        //targettedMount.transform.rotation = this.transform.rotation;
        //targettedMount.transform.SetParent(this.transform);

        if (OnMount != null)
            OnMount();

        yield return null;
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, mat);
    //}
}
