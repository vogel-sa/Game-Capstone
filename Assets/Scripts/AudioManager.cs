using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	private AudioSource source;


	public AudioClip clip;
	public float volume;



	public void Start() {
		source = GetComponent<AudioSource>();
		volume = 1.0f;
	}

	public void playSoundEffect(AudioClip soundEffect) {
		Debug.Log ("sounds");
		clip = soundEffect;
		source.clip = clip;
		source.volume = volume;
		source.Play();
	}


}
