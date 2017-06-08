using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour {

    public PlayerMount follower;
    public Animator anim;

    private void Start()
    {
        if(!anim)
            anim = GetComponent<Animator>();
    }
    // Use this for initialization
    void OnEnable () {
        PerspectiveSwitcher.OnMount += StopAnimating;
        PerspectiveSwitcher.OnMount += StartAnimating;
    }
	
	// Update is called once per frame
	void OnDisable () {
        PerspectiveSwitcher.OnMount -= StopAnimating;
        PerspectiveSwitcher.OnMount -= StartAnimating;
    }

    void StopAnimating()
    {
        Debug.Log("StopAnimating Called");
        if (follower.isMounted)
            if (anim != null)
                anim.enabled = false;
            
    }

    void StartAnimating()
    {
        Debug.Log("StartAnimating Called");
        if (!follower.isMounted)
            if (anim != null)
                anim.enabled = true;
    }
}
