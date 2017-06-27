using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionFilter : MonoBehaviour {

    public PlayerMount[] mounts;
    int val;
    public PerspectiveSwitcherBlink blink;
	// Use this for initialization
	void Awake () {
        mounts = FindObjectsOfType<PlayerMount>();
        val = -1;
        blink = FindObjectOfType<PerspectiveSwitcherBlink>();
	}

    private void Update()
    {
        val = -1;
        for(int i = 0; i < mounts.Length; i++)
        {
            if (mounts[i].isMounted)
                val = i;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //if (val > -1)
        //    Graphics.Blit(source, destination, mounts[val].filter);
        //else
        //    Graphics.Blit(source, destination);

        Graphics.Blit(source, destination, blink.mat);
    }

}
