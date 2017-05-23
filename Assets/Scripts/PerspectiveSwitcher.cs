using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

public class PerspectiveSwitcher : MonoBehaviour {

    private PlayerMount[] mounts;

    public float transferSpeed;
    public float loadSpeed;
    public Image reticle;
    private Transform target;
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
        StopCoroutine(lastCoroutine);
        reticle.fillAmount = 0;
    }

    void startLoad()
    {
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
        stopLoad();
        StartCoroutine(switchPerspective());
    }

    IEnumerator switchPerspective()
    {
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
            transform.position = Vector3.Lerp(transform.position, target.position, transferSpeed);
            dist = (target.position - transform.position).magnitude;
            yield return null;
        }
    }
}
