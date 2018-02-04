using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using cakeslice;

public class Abilities : MonoBehaviour {

	[SerializeField]
	private Button[] buttons;

    void OnEnable()
    {
		GetComponent<PlayerMovementManager>().OnSelect += EnableButtons;
    }

    void OnDisable()
    {
		GetComponent<PlayerMovementManager>().OnSelect -= EnableButtons;
    }

    private void EnableButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void DisableButtons()
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }
    }

    public void BasicShootAbility(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_basicShootAbility(stats)); }
    private IEnumerator _basicShootAbility(PlayerCharacterStats stats)
    {
        LineRenderer lineRenderer = null;
        try
        {
            DisableButtons();
            var abilData = (from abil in stats.AbilityData where abil.Name == "Shoot" select abil).FirstOrDefault();
            Debug.Log(abilData.Description);
			var pmm = GetComponent<PlayerMovementManager>();
			pmm.SetQuadsEnabled(false);
			pmm.enabled = false;
            var range = abilData.OtherValues.Range;
            var width = abilData.OtherValues.Width;
            Vector3 direction = Vector3.zero;
            RaycastHit hit;
            lineRenderer = new GameObject().AddComponent<LineRenderer>();
            lineRenderer.material = Resources.Load<Material>("UI");
            lineRenderer.endWidth = lineRenderer.startWidth = width;
            do
            {
                if (Input.GetKeyDown(KeyCode.Escape)) yield break;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    var mousePos = new Vector3(hit.point.x, stats.transform.position.y + 1/*aimLine.transform.position.y*/, hit.point.z);
                    var origin = stats.transform.position + Vector3.up;
                    direction = origin - mousePos;
                    lineRenderer.SetPositions(new Vector3[] { origin - direction.normalized * .5f, origin - direction.normalized*range });
					stats.transform.LookAt(new Vector3((origin - direction.normalized*range).x, stats.transform.position.y, (origin - direction.normalized*range).z));
                }
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            lineRenderer.gameObject.SetActive(false);
            // TODO: Animation of ability
            EnemyStats hitStats;
            if (Physics.Raycast(stats.transform.position + Vector3.up, -(direction.normalized), out hit, range, ~LayerMask.GetMask("Player", "Ground", "Ignore Raycast", "Flare")))//, LayerMask.NameToLayer("Enemy")))
            {
				if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Enemy")
                {
                    hitStats = hit.transform.parent.GetComponent<EnemyStats>();
                    hitStats.TakeDamage(abilData.DamageAmount);
                }
            }
            Debug.Log("Bang");
			GetComponent<TurnManager>().AutoEndTurnCheck();
            abilData.Currcooldown = abilData.Maxcooldown;
            stats.hasMoved = true;
            stats.Actionsleft--;
			GetComponent<TurnManager>().AutoEndTurnCheck();
            yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        }
        finally
        {
            //GetComponent<PlayerMovementManager>().SetQuadsEnabled(true);
			GetComponent<PlayerMovementManager>().enabled = true;
            //GetComponent<PlayerMovementManager>().Select(stats.transform, stats);
            if (lineRenderer) Destroy(lineRenderer.gameObject);
			if (stats.Actionsleft == 0)
				GetComponent<PlayerMovementManager>().Deselect ();
			else
				GetComponent<PlayerMovementManager>().Select (stats);
            EnableButtons();
        }
    }

    public void RoadFlareAbility(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_roadFlareAbility(stats)); }
    private IEnumerator _roadFlareAbility(PlayerCharacterStats stats)
    {
        try
        {
            DisableButtons();
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
            Vector3 to = Vector3.zero;
            do
            {
                if (Input.GetKeyDown(KeyCode.Escape)) yield break;
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            Debug.Log("flare dropped");
            GameObject flare = Instantiate(Resources.Load<GameObject>("Prefabs/Flare"));
            flare.transform.position = stats.transform.position + Vector3.up + stats.transform.forward * .7f;
            stats.hasMoved = true;
            stats.Actionsleft--;

            var ability = (from a in stats.AbilityData
                           where a.Name == "Flare"
                           select a).FirstOrDefault();
             ability.Currcooldown = ability.Maxcooldown;
			GetComponent<TurnManager>().AutoEndTurnCheck();
            yield return null;
        }
        finally
        {
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(true);
			GetComponent<PlayerMovementManager>().enabled = true;
			if (stats.Actionsleft == 0)
				GetComponent<PlayerMovementManager>().Deselect ();
			else
				GetComponent<PlayerMovementManager>().Select (stats);
            EnableButtons();
        }
    }

    public void FlashlightAbility(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_flashlightAbility(stats)); }
    private IEnumerator _flashlightAbility(PlayerCharacterStats stats)
    {
        LineOfSight los = null;
        RaycastIfInLightCone flashlight = null;
        try
        {
            DisableButtons();
            var abilData = (from abil in stats.AbilityData where abil.Name == "Flashlight" select abil).FirstOrDefault();
            Debug.Log(abilData.Description);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
            var range = abilData.OtherValues.Range;
            var width = abilData.OtherValues.Width;
            Vector3 direction = Vector3.zero;
            RaycastHit hit;
            los = new GameObject().AddComponent<LineOfSight>();
			los.gameObject.AddComponent<cakeslice.Outline>();
            los._idle = Resources.Load<Material>("Clear");
            los.transform.position = stats.transform.position + Vector3.up;
            los._cullingMask = LayerMask.GetMask("Obstacle");
            los._maxAngle = (int)abilData.OtherValues.Angle;
            los._maxDistance = abilData.OtherValues.Range;
            flashlight = new GameObject().AddComponent<RaycastIfInLightCone>();//stats.transform.Find("Spotlight");
            yield return null; // Wait for flashlight to initialize.
            flashlight.Range = abilData.OtherValues.Range;
            flashlight.Angle = (int)abilData.OtherValues.Angle;
            flashlight.transform.position = stats.transform.position + Vector3.up;
            flashlight.transform.parent = stats.transform;
            flashlight.gameObject.SetActive(false);
            flashlight.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            do
            {
                if (Input.GetKeyDown(KeyCode.Escape)) yield break;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                {
                    var mousePos = new Vector3(hit.point.x, stats.transform.position.y + 1/*aimLine.transform.position.y*/, hit.point.z);
                    var origin = stats.transform.position + Vector3.up;
                    direction = origin - mousePos;
                    var lookAt = new Vector3((origin - direction.normalized * range).x, stats.transform.position.y/* + 1*/, (origin - direction.normalized * range).z);
                    lookAt = new Vector3(lookAt.x, los.transform.position.y, lookAt.z);
                    los.transform.LookAt(lookAt);
					stats.transform.LookAt(lookAt + Vector3.down);
                    flashlight.transform.LookAt(lookAt);
                    //flashlight.transform.LookAt(lookAt);
                }
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            los.gameObject.SetActive(false);
            flashlight.gameObject.SetActive(true);
            // TODO: Animation of ability

            abilData.Currcooldown = abilData.Maxcooldown;
            stats.hasMoved = true;
            stats.Actionsleft--;
			GetComponent<TurnManager>().AutoEndTurnCheck();
            yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        }
        finally
        {
            //GetComponent<PlayerMovementManager>().SetQuadsEnabled(true);
			GetComponent<PlayerMovementManager>().enabled = true;
            if (los) Destroy(los.gameObject);
            if (flashlight && !flashlight.gameObject.activeSelf) Destroy(flashlight.gameObject);
			if (stats.Actionsleft == 0)
				GetComponent<PlayerMovementManager>().Deselect ();
			else
				GetComponent<PlayerMovementManager>().Select (stats);
            EnableButtons();
        }
    }
}
