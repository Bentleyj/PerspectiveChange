using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;


// Written by James Bentley 21th May 2017
// Really simple class that wraps some fields that I was easy access to. This cannot extend VRInteractiveItem because
// We want the item to be attached to the avatar not to the mount and we want this to be attached to the mount
public class PlayerMount : MonoBehaviour {

    public GameObject avatar;
    public VRInteractiveItem interactiveItem;
    public bool isMounted;

	// Use this for initialization
	void Start () {
        // Find the VRInteractiveItem in the children
        if(!(interactiveItem = avatar.GetComponent<VRInteractiveItem>()))
            interactiveItem = avatar.AddComponent<VRInteractiveItem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (isMounted)
            avatar.SetActive(false);
        else
            avatar.SetActive(true);
	}
}
