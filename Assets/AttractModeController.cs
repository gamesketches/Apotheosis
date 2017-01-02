using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent (typeof (AudioSource))]

public class AttractModeController : MonoBehaviour {

	public MovieTexture movie;
	// Use this for initialization
	void Start () {
		movie = Resources.Load<MovieTexture>("AttractModeVideo");
		GetComponent<Renderer>().materials[0].mainTexture = movie;
		movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
		if(!movie.isPlaying) {
			SceneManager.LoadScene(0);
		}
	}
}
