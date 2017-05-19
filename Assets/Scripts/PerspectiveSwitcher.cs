using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class PerspectiveSwitcher : MonoBehaviour {

    public VRInteractiveItem item;

	// Use this for initialization
	void Start () {
        item.OnOver += switchPerspective;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void switchPerspective()
    {
        Debug.Log("I've been called!");
    }
}
