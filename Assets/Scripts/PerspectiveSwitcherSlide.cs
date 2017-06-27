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
// The PlayerMount script sits on a prefab in my scene which includes a VRInteractiveItem as well as the PlayerMount script whcih hold several fields that are useful. 
public class PerspectiveSwitcherSlide : PerspectiveSwitcher {

    public new static event MountAction OnMount;

    // Use this for initialization
    new protected void Start () {
        // Here we find all the mPlayerMount objects in the scene and add our callbacks to their events.
        base.Start();
    }

    // Here's the coroutine which moves the player to the new perspective
    new protected IEnumerator switchPerspective(PlayerMount targettedMount)
    {
        Transform target = targettedMount.transform;
        float dist = (target.position - transform.position).magnitude;
        while (dist > 0.4)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, transferSpeed);
            targettedMount.transform.rotation = Quaternion.Lerp(targettedMount.transform.rotation, this.transform.rotation, transferSpeed);
            //transform.rotation = target.rotation;// Quaternion.Lerp(transform.rotation, target.rotation, transferSpeed);
            dist = (target.position - transform.position).magnitude;
            //targettedMount.avatarMaterial.SetColor("Albedo",)
            yield return null;
        }
        //yield return null;
        targettedMount.isMounted = true;
        //targettedMount.transform.LookAt(this.transform.position + this.transform.forward);
        for (int i = 0; i < targettedMount.mountableAvatarParts.Length; i++)
        {
            targettedMount.mountableAvatarParts[i].SetActive(false);
        }
        //yield return null;
        targettedMount.transform.rotation = this.transform.rotation;
        targettedMount.transform.SetParent(this.transform);

        base.switchPerspective();
    }
}
