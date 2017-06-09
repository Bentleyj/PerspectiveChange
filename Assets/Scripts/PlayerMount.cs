using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;


// Written by James Bentley 21th May 2017
// Really simple class that wraps some fields that I was easy access to. This cannot extend VRInteractiveItem because
// We want the item to be attached to the avatar not to the mount and we want this to be attached to the mount
public class PlayerMount : MonoBehaviour {

    public GameObject avatar;
    public GameObject[] mountableAvatarParts;
    public VRInteractiveItem interactiveItem;
    public Material AvatarMaterial;
    [HideInInspector]
    public bool isMounted;
    public Transform originalAvatarTransform;

    public Material filter;

	// Use this for initialization
	void Start () {
        // Find the VRInteractiveItem in the children
        if(!(interactiveItem = avatar.GetComponent<VRInteractiveItem>()))
            interactiveItem = avatar.AddComponent<VRInteractiveItem>();
        if (avatar)
            originalAvatarTransform = avatar.transform;
        Debug.Log("Avatar Material: " + AvatarMaterial);

    }

    private void OnEnable()
    {
        PerspectiveSwitcher.OnMount += setupMount;
        PerspectiveSwitcher.OnStartSwitch += leaveMount;
    }


    private void OnDisable()
    {
        PerspectiveSwitcher.OnMount -= setupMount;
        PerspectiveSwitcher.OnStartSwitch -= leaveMount;
    }

    void setupMount()
    {
        //this.avatar.transform.SetParent(this.transform);
    }

    void leaveMount()
    {
        for (int i = 0; i < mountableAvatarParts.Length; i++)
        {
            mountableAvatarParts[i].SetActive(true);
        }
        Color col = AvatarMaterial.GetColor("_Color");
        col.a = 1;
        AvatarMaterial.SetColor("_Color", col);
        //this.avatar.transform.rotation = originalAvatarTransform.rotation;
        this.transform.parent = null;
        //this.transform.position = originalAvatarTransform.position;
        //this.transform.rotation = originalAvatarTransform.rotation;
    }
}
