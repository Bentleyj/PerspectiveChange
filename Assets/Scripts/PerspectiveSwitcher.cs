using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class PerspectiveSwitcher : MonoBehaviour {

    public PlayerMount[] mounts;

    public float transferSpeed;
    public float loadSpeed;
    private Transform target;
    public Image reticle;
    private Coroutine lastCoroutine;

	// Use this for initialization
	void Start () {
        mounts = FindObjectsOfType<PlayerMount>();
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver += startLoad;
            mounts[i].interactiveItem.OnOut += stopLoad;
        }

    }

    private void OnDisable()
    {
        for (int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver -= startLoad;
            mounts[i].interactiveItem.OnOut -= stopLoad;
        }
    }

    // Update is called once per frame
    void Update () {

    }

    void stopLoad()
    {
        Debug.Log("stopLoad Called!");
        StopCoroutine(lastCoroutine);
        reticle.fillAmount = 0;
    }

    void startLoad()
    {
        Debug.Log("startLoad Called!");
        lastCoroutine = StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        while(reticle.fillAmount < 1.0)
        {
            reticle.fillAmount += loadSpeed;
            yield return null;
        }
        startSwitch();
    }

    void startSwitch()
    {
        Debug.Log("startSwitch Called!");
        stopLoad();
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
            transform.position = Vector3.Lerp(transform.position, target.position, transferSpeed);
            dist = (target.position - transform.position).magnitude;
            yield return null;
        }
    }
}
