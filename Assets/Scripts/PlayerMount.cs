using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class PlayerMount : MonoBehaviour {

    public GameObject avatar;
    public VRInteractiveItem interactiveItem;

	// Use this for initialization
	void Start () {
        if(avatar.GetComponent<VRInteractiveItem>() == null)
            interactiveItem = avatar.AddComponent<VRInteractiveItem>();
        //t = new Transform(transform);
        //t.position = new Vector3(t.position.x, t.position.y + 5, t.position.z);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
