using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	// Use this for initialization
	public int sceneIndex;

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadScene()
	{
		SceneManager.LoadScene (sceneIndex);
	}
}
