using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

	public delegate void ManagerUnloader();
	public static event ManagerUnloader UnloadLevelEvent;

	void Awake()
	{
		SceneManager.sceneUnloaded += Unload;
	}

	void Unload(Scene thisScene)
	{
		SceneManager.sceneUnloaded -= Unload;
		UnloadLevelEvent ();
	}
}
