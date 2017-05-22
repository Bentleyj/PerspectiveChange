using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;

public class PerspectiveSwitcher : MonoBehaviour {

    public PlayerMount[] mounts;

    public float speed;
    private Transform target;

	// Use this for initialization
	void Start () {
        mounts = FindObjectsOfType<PlayerMount>();
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver += startSwitch;
            //.OnOver += startSwitch;
        }

    }

    private void OnDisable()
    {
        for (int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver -= startSwitch;
        }
    }

    // Update is called once per frame
    void Update () {
        //for (int i = 0; i < items.Length; i++)
        //{
        //    Debug.Log(i + ": " + items[i].GetComponentInChildren<VRInteractiveItem>().IsOver);
        //}
    }
    
    void startSwitch()
    {
        Debug.Log("startSwitch called!");
        StartCoroutine(switchPerspective());
    }

    IEnumerator switchPerspective()
    {
        Debug.Log("Switching!");
        for (int i = 0; i < mounts.Length; i++)
        {
            if (mounts[i].interactiveItem.IsOver)
            {
                target = mounts[i].transform;
                break;
            }

        }
        float dist = (target.position - transform.position).magnitude;
        while (dist > 0.01)
        {
            Debug.Log("Going!");
            transform.position = Vector3.Lerp(transform.position, target.position, speed);
            dist = (target.position - transform.position).magnitude;
            yield return null;
        }
    }
}
