using UnityEngine;
using System.Collections;

public class SoundFade : MonoBehaviour {

	AudioSource audio;
	public bool playAudio = true;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (playAudio) {
			if (audio.volume < 1)
				audio.volume += Time.deltaTime / 5;
		} else {
			if(audio.volume > 0)
				audio.volume -= Time.deltaTime;
			
		}

	}
}
