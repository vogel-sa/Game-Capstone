using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {
	[SerializeField]
	private string ScenePath;
	private GameObject scene;
	void Awake()
	{
		SceneManager.sceneUnloaded += Unload;
		scene = Instantiate (Resources.Load<GameObject> (ScenePath));
	}

	void Unload(Scene thisScene)
	{
		SceneManager.sceneUnloaded -= Unload;
		Destroy (scene);
		print ("destroyed");
	}
}
