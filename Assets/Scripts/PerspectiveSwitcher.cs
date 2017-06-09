using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.UI;

// Written by James Bentley 19th May 2017
// Last updated 23rd of May 2017
// The intention of this class is to be attached to a camera which allows 
// the camera to switch it's position between various objects in the scene
// All the coroutines make it look scary but it's actually pretty simple!

// Basically This class subscribes to events called by VRInteractiveObjects in the scene. 
// The PlayerMount script sits on a prefab in my scene which includes a VRInteractiveItem as well as the PlayerMount script whcih hold several fields that are useful. 
public class PerspectiveSwitcher : MonoBehaviour {

    private PlayerMount[] mounts;   // A list of all the PlayerMount objects in the scene. PlayerMounts are what we can sit atop.
    public float transferSpeed;     // The speed at which you teleport to the location.
    public float loadSpeed;         // The time you need to focus on any mount before your perspective is changed to it.
    public Image reticle;           // A Reference to the targetting Reticle Image that we need to fill up.
    private Coroutine lastCoroutine; // A Reference to the last coroutine we called (the load coroutine) so we can kill it if we need to
    public delegate void MountAction();
    public static event MountAction OnMount;
    public AudioSource audio;

    public delegate void SwitchAction();
    public static event SwitchAction OnStartSwitch;

    // Use this for initialization
    void Start () {
        // Here we find all the mPlayerMount objects in the scene and add our callbacks to their events.
        mounts = FindObjectsOfType<PlayerMount>();
        for(int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver += startLoad;
            mounts[i].interactiveItem.OnOut += stopLoad;
        }

        if (!audio)
            audio = FindObjectOfType<AudioSource>();
    }

    // This is called when we disbale our object
    private void OnDisable()
    {
        //Make sure we remove the callbacks so we don't get a memory leak.
        for (int i = 0; i < mounts.Length; i++)
        {
            mounts[i].interactiveItem.OnOver -= startLoad;
            mounts[i].interactiveItem.OnOut -= stopLoad;
        }
    }

    // Update is called once per frame
    void Update () {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 10);
    }

    // This method stops the "Load" method which is called when the OnOut event.
    public void stopLoad()
    {
        StopCoroutine(lastCoroutine);
        reticle.fillAmount = 0;
    }

    // Starts the coroutine that starts a timer which counts down until we transition to the new mount position
    public void startLoad()
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

    // This is the coroutine which Loads up the reticle. Once the while loop is finished we switch our perspective to the new mount
    private IEnumerator Load(PlayerMount targettedMount)
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

    // This kicks off hte coroutine which changes the perspective
    public void startSwitch(PlayerMount targettedMount)
    {
        stopLoad();
        OnStartSwitch();
        audio.Play();
        StartCoroutine(switchPerspective(targettedMount));
    }

    // Here's the coroutine which moves the player to the new perspective
    private IEnumerator switchPerspective(PlayerMount targettedMount)
    {
        Transform target = targettedMount.transform;
        float dist = (target.position - transform.position).magnitude;
        Material mat = targettedMount.AvatarMaterial;
        while (dist > 0.4)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, transferSpeed);
            targettedMount.transform.rotation = Quaternion.Lerp(targettedMount.transform.rotation, this.transform.rotation, transferSpeed);
            //transform.rotation = target.rotation;// Quaternion.Lerp(transform.rotation, target.rotation, transferSpeed);
            dist = (target.position - transform.position).magnitude;
            Color col = mat.GetColor("_Color");
            col.a = Mathf.Lerp(col.a, 0, transferSpeed/2);
            mat.SetColor("_Color", col);
            //targettedMount.avatarMaterial.SetColor("Albedo",)
            yield return null;
        }
        //yield return null;
        targettedMount.isMounted = true;
        //targettedMount.transform.LookAt(this.transform.position + this.transform.forward);
        for (int i = 0; i < targettedMount.mountableAvatarParts.Length; i++)
        {
            targettedMount.mountableAvatarParts[i].SetActive(false);
        }
        //yield return null;
        targettedMount.transform.rotation = this.transform.rotation;
        targettedMount.transform.SetParent(this.transform);

        if (OnMount != null)
            OnMount();
    }

    // This is a utility function that gets the current targetted mount using it/s "IsOver" field.
    private PlayerMount getTargettedMount()
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
