using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioSource source;

	public AudioClip clip;


	public void Start() {
		source = GetComponent<AudioSource>();
	}

	public void playSoundEffect(AudioClip soundEffect) {
		Debug.Log ("sounds");
		clip = soundEffect;
		source.clip = clip;
		source.Play();
	}


}
