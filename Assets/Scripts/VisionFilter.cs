using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionFilter : MonoBehaviour {

    public PlayerMount[] mounts;
    public Material mat;
	// Use this for initialization
	void Awake () {
        mounts = FindObjectsOfType<PlayerMount>();
	}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, mounts[0].filter);
    }

}
