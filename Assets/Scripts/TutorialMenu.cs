using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : MonoBehaviour {

	[SerializeField]
	private Camera objectiveCam;
	private Camera mainCam;
	[SerializeField]
	private float cameraSpeed = 1f;
	[SerializeField]
	private bool nextEnabledFromStart = true;


	[SerializeField]
	private TutorialMenu next;
	[SerializeField]
	private Button nextButton;


	private Vector3 startCameraLocation;
	[SerializeField]
	private PlayerMovementManager playerSelector;
	[SerializeField]
	private CollisionEventBroadcast collisionActivator;

	// Use this for initialization
	void Awake ()
	{
		startCameraLocation = Camera.main.transform.position;
		if (collisionActivator) {
			collisionActivator.onTriggerEnter += enableThis;
			gameObject.SetActive (false);
		}
		nextButton = GetComponentInChildren<Button> ();
		nextButton.onClick.AddListener(() => {
			gameObject.SetActive (false);
		});
		if (objectiveCam)
		{
			nextButton.onClick.AddListener(() => {
				Debug.Log("Returning camera");
				//LeanTween.move(cameraHolder, startCameraLocation, cameraSpeed)
				//	.setOnComplete(() => {if (next) next.gameObject.SetActive(true);});
				Camera.main.enabled = true;
				objectiveCam.gameObject.SetActive (false);
				objectiveCam.enabled = false;
			});
		}
		else if (next)
		{
			nextButton.onClick.AddListener(() => {
				next.gameObject.SetActive(true);
			});
		}
	}

	void Start()
	{
		if (!nextEnabledFromStart)
			nextButton.gameObject.SetActive (false);
		else
			nextButton.gameObject.SetActive (true);
	}

	void OnEnable()
	{
		if (playerSelector) playerSelector.OnSelect += enableNextButton;
		if (objectiveCam) {
			Debug.Log("Moving camera");
			//LeanTween.move (cameraHolder, cameraLocation.position, cameraSpeed)
			//	.setOnComplete(() => enableNextButton(null));
			Camera.main.enabled = true;
			objectiveCam.gameObject.SetActive (true);
			objectiveCam.enabled = true;
			enableNextButton (null);
		}
	}

	void OnDisable()
	{
		if (playerSelector) playerSelector.OnSelect -= enableNextButton;
	}

	void enableThis(Collider col)
	{
		Debug.Log ("Menu triggered");
		gameObject.SetActive (true);
		collisionActivator.onTriggerEnter -= enableThis;
	}

	void enableNextButton(PlayerCharacterStats stats)
	{
		nextButton.gameObject.SetActive (true);
	}
}
