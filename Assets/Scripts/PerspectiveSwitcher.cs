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
        PlayerMount targettedMount = getTargettedMount();
        if(targettedMount)
        {
            if (!targettedMount.isMounted)
            {
                lastCoroutine = StartCoroutine(Load(targettedMount));
            }
        }
    }

    IEnumerator Load(PlayerMount targettedMount)
    {
        while(reticle.fillAmount < 1.0)
        {
            reticle.fillAmount += loadSpeed;
            yield return null;
        }
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].isMounted = false;
        }
        startSwitch(targettedMount);
    }

    void startSwitch(PlayerMount targettedMount)
    {
        stopLoad();
        StartCoroutine(switchPerspective(targettedMount));
    }

    IEnumerator switchPerspective(PlayerMount targettedMount)
    {
        Transform target = targettedMount.transform;
        float dist = (target.position - transform.position).magnitude;
        while (dist > 0.01)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, transferSpeed);
            dist = (target.position - transform.position).magnitude;
            yield return null;
        }
        targettedMount.isMounted = true;
    }

    PlayerMount getTargettedMount()
    {
        for (int i = 0; i < mounts.Length; i++)
        {
            if (mounts[i].interactiveItem.IsOver)
            {
                return mounts[i];
            }
        }
        return null;
    }
}
