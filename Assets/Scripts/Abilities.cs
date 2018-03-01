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

    private void EnableButtons(PlayerCharacterStats stats)
    {
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(true);
        }
    }

    private void DisableButtons(PlayerCharacterStats stats)
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
            DisableButtons(stats);
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
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
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
            genericStatChange(abilData, stats);
            yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        }
        finally
        {
            genericCleanup(stats);
            if (lineRenderer) Destroy(lineRenderer.gameObject);
        }
    }

    public void RoadFlareAbility(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_roadFlareAbility(stats)); }
    private IEnumerator _roadFlareAbility(PlayerCharacterStats stats)
    {
        LineOfSight los = null;
        try
        {
            
            var abilData = (from abil in stats.AbilityData where abil.Name == "Flare" select abil).FirstOrDefault();
            DisableButtons(stats);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
            Vector3 direction = Vector3.zero;
            RaycastHit hit;
            Vector3 hitpoint = Vector3.zero;
            Vector3 mousePos = new Vector3(0,0,0);
            los = new GameObject().AddComponent<LineOfSight>();
            los._maxAngle = (int)abilData.OtherValues.Angle;
            los._maxDistance = abilData.OtherValues.Range;
            los.gameObject.AddComponent<cakeslice.Outline>();
            los._idle = Resources.Load<Material>("Clear");
            los.transform.position = stats.transform.position + Vector3.up * .2f;
            los._cullingMask = LayerMask.GetMask("Obstacle");
            bool goodHit = false;
            float distance = 0;
            do
            {
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
                    {
                        hitpoint = hit.point;
                        Vector3 hitPointAtEyeLevel = new Vector3(hit.point.x, stats.transform.position.y + 1, hit.point.z);
                        Ray losRay = new Ray(stats.transform.position, hitPointAtEyeLevel - stats.transform.position);
                        RaycastHit newhit;
                        if (!Physics.Raycast(losRay, out newhit, Vector3.Distance(stats.transform.position + Vector3.up, hitPointAtEyeLevel), LayerMask.GetMask("Obstacle")))
                        {
                            print("raycsat didn't hit");
                            goodHit = true;
                            var origin = stats.transform.position + Vector3.up;
                            origin.y = 0;
                            distance = Vector3.Distance(origin, hitPointAtEyeLevel);
                            break;
                        }


                    }
                }
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
                yield return null;
            } while (!(Input.GetMouseButtonDown(0) && distance < los._maxDistance && goodHit));
            Debug.Log("flare dropped");
            GameObject flare = Instantiate(Resources.Load<GameObject>("Prefabs/Flare"));
            mousePos.y = 1.1f;
            flare.transform.position = hitpoint;//mousePos;

            genericStatChange(abilData, stats);
            yield return null;
        }
        finally
        {
            genericCleanup(stats);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(true);
			GetComponent<PlayerMovementManager>().enabled = true;
            if (los) Destroy(los.gameObject);
        }
    }

    public void FlashlightAbility(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_flashlightAbility(stats)); }
    private IEnumerator _flashlightAbility(PlayerCharacterStats stats)
    {
        LineOfSight los = null;
        RaycastIfInLightCone flashlight = null;
        try
        {
            DisableButtons(stats);
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
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
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
            genericStatChange(abilData,stats);
            stats.hasMoved = true;
            yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        }
        finally
        {
            genericCleanup(stats);
            if (los) Destroy(los.gameObject);
            if (flashlight && !flashlight.gameObject.activeSelf) Destroy(flashlight.gameObject);
        }
    }

	public void CoveringFire(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_CoveringFire(stats)); }
	private IEnumerator _CoveringFire(PlayerCharacterStats stats)
	{
		LineOfSight los = null;
		try {
			DisableButtons(stats);
			var abilData = (from abil in stats.AbilityData where abil.Name == "CoveringFire" select abil).FirstOrDefault();
			//Debug.Log(abilData.Description);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
			los = new GameObject().AddComponent<LineOfSight>();
			los.gameObject.AddComponent<cakeslice.Outline>();
			los._idle = Resources.Load<Material>("Clear");
			los.transform.position = stats.transform.position + Vector3.up;
			los._cullingMask = LayerMask.GetMask("Obstacle");
			los._maxAngle = (int)abilData.OtherValues.Angle;
			los._maxDistance = abilData.OtherValues.Range;
			do
			{
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
				yield return null;
			} while (!Input.GetMouseButtonDown(0));
			los.gameObject.SetActive(false);
			stats.isCoveringFire = true;
			stats.hasMoved = true;
            genericStatChange(abilData, stats);
			yield return new WaitForSeconds(.5f);
			//abiltiy effects in enemyturn
		}
		finally {
            genericCleanup(stats);
			if (los) Destroy(los.gameObject);
		}
			
	}

	public void Fortify(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_Fortify(stats)); }
	private IEnumerator _Fortify(PlayerCharacterStats stats)
	{
		try {
			DisableButtons(stats);
			var abilData = (from abil in stats.AbilityData where abil.Name == "Fortify" select abil).FirstOrDefault();
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
			do
			{
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
				yield return null;
			} while (!Input.GetMouseButtonDown(0));
			stats.isFortifying = true;
            genericStatChange(abilData, stats);
			stats.MitigationValue += abilData.DamageAmount;
			yield return new WaitForSeconds(.5f);
			//Wear off happens in player on turn start
			print("Fortifying");
		}
		finally {
            genericCleanup(stats);
		}

	}

	public void RapidFire(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_RapidFire(stats)); }
	private IEnumerator _RapidFire(PlayerCharacterStats stats)
	{
		LineOfSight cone = null;
		try
		{
			var abilData = (from abil in stats.AbilityData where abil.Name == "RapidFire" select abil).FirstOrDefault();
			Debug.Log(abilData.Description);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
			var range = abilData.OtherValues.Range;
			var width = abilData.OtherValues.Width;
			Vector3 direction = Vector3.zero;
			RaycastHit hit;
			cone = new GameObject().AddComponent<LineOfSight>();
			cone.gameObject.AddComponent<cakeslice.Outline>();
			cone._idle = Resources.Load<Material>("Clear");
			cone.transform.position = stats.transform.position + Vector3.up;
			cone._cullingMask = LayerMask.GetMask("Obstacle");
			cone._maxAngle = (int)abilData.OtherValues.Angle;
			cone._maxDistance = abilData.OtherValues.Range;
			do
			{
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
				{
					var mousePos = new Vector3(hit.point.x, stats.transform.position.y + 1/*aimLine.transform.position.y*/, hit.point.z);
					var origin = stats.transform.position + Vector3.up;
					direction = origin - mousePos;
					var directedAt = new Vector3((origin - direction.normalized * range).x, stats.transform.position.y/* + 1*/, (origin - direction.normalized * range).z);
					directedAt = new Vector3(directedAt.x, cone.transform.position.y, directedAt.z);
					stats.transform.LookAt(directedAt + Vector3.down);
					cone.transform.LookAt(directedAt);
				}
				yield return null;
			} while (!Input.GetMouseButtonDown(0));
			cone.gameObject.SetActive(false);
			EnemyStats hitStats;
			Quaternion startingAngle = Quaternion.AngleAxis(-(cone._maxAngle/2), Vector3.up);
			int increment = 2;
			Quaternion stepAngle = Quaternion.AngleAxis(increment, Vector3.up);
			var angle = cone.transform.rotation * startingAngle;
			var direction2 = angle * Vector3.forward;
			var pos = cone.transform.position;
			List<EnemyStats> enemies = new List<EnemyStats>();
			for (var i = 0; i < (cone._maxAngle/increment); i++) {
				if (Physics.Raycast(stats.transform.position + Vector3.up, direction2, out hit, range, ~LayerMask.GetMask("Player", "Ground", "Ignore Raycast", "Flare")))//, LayerMask.NameToLayer("Enemy")))
				{
					if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Enemy")
					{
						hitStats = hit.transform.parent.GetComponent<EnemyStats>();
						if (!enemies.Contains(hitStats)) {
							enemies.Add(hitStats);
						}
						if (!hitStats.hitByAbility()) {

							hitStats.TakeDamage(abilData.DamageAmount);

							hitStats.swapFlag();
						}
					}
				}
				direction2 = stepAngle * direction2;
			}

			Debug.Log("RapidFire");
			foreach (EnemyStats stat in enemies) {
				stat.swapFlag();

			}
            genericStatChange(abilData, stats);
			yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
		}
		finally
		{
            genericCleanup(stats);
			if (cone) Destroy(cone.gameObject);
		}
	}

	public void shotgunBlast(PlayerCharacterStats stats) { if (stats.Actionsleft > 0) StartCoroutine(_shotgunBlast(stats)); }
	private IEnumerator _shotgunBlast(PlayerCharacterStats stats)
	{
		LineOfSight cone = null;
		try
		{
			var abilData = (from abil in stats.AbilityData where abil.Name == "Shotgun Blast" select abil).FirstOrDefault();
			Debug.Log(abilData.Description);
			GetComponent<PlayerMovementManager>().SetQuadsEnabled(false);
			GetComponent<PlayerMovementManager>().enabled = false;
			var range = abilData.OtherValues.Range;
			var width = abilData.OtherValues.Width;
			Vector3 direction = Vector3.zero;
			RaycastHit hit;
			cone = new GameObject().AddComponent<LineOfSight>();
			cone.gameObject.AddComponent<cakeslice.Outline>();
			cone._idle = Resources.Load<Material>("Clear");
			cone.transform.position = stats.transform.position + Vector3.up;
			cone._cullingMask = LayerMask.GetMask("Obstacle");
			cone._maxAngle = (int)abilData.OtherValues.Angle;
			cone._maxDistance = abilData.OtherValues.Range;


			do
			{
				if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1)) yield break;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
				{
					var mousePos = new Vector3(hit.point.x, stats.transform.position.y + 1/*aimLine.transform.position.y*/, hit.point.z);
					var origin = stats.transform.position + Vector3.up;
					direction = origin - mousePos;
					var directedAt = new Vector3((origin - direction.normalized * range).x, stats.transform.position.y + 1, (origin - direction.normalized * range).z);
					directedAt = new Vector3(directedAt.x, cone.transform.position.y, directedAt.z);
					stats.transform.LookAt(directedAt + Vector3.down);
					cone.transform.LookAt(directedAt);

				}
				yield return null;
			} while (!Input.GetMouseButtonDown(0));
			cone.gameObject.SetActive(false);
			EnemyStats hitStats;
			Quaternion startingAngle = Quaternion.AngleAxis(-(cone._maxAngle/2), Vector3.up);
			int increment = 8;
			Quaternion stepAngle = Quaternion.AngleAxis(increment, Vector3.up);
			var angle = cone.transform.rotation * startingAngle;
			var direction2 = angle * Vector3.forward;
			var pos = cone.transform.position;
			List<EnemyStats> enemies = new List<EnemyStats>();
			for (var i = 0; i < (cone._maxAngle/increment); i++) {
				if (Physics.Raycast(stats.transform.position + Vector3.up, direction2, out hit, range, ~LayerMask.GetMask("Player", "Ground", "Ignore Raycast", "Flare")))//, LayerMask.NameToLayer("Enemy")))
				{
					if (LayerMask.LayerToName(hit.transform.gameObject.layer) == "Enemy")
					{
						hitStats = hit.transform.parent.GetComponent<EnemyStats>();
						if (!enemies.Contains(hitStats)) {
							enemies.Add(hitStats);
						}
						if (!hitStats.hitByAbility()) {
							if (hit.distance <= abilData.OtherValues.InitialRange) {
								hitStats.TakeDamage(abilData.DamageAmount);
							}
							else {
								hitStats.TakeDamage(abilData.DamageAmount - abilData.OtherValues.FallOffDamage);
							}
							hitStats.swapFlag();
						}
					}
				}
				direction2 = stepAngle * direction2;
			}

			foreach (EnemyStats stat in enemies) {
				stat.swapFlag();
			}
            genericStatChange(abilData, stats);
			yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
		}
		finally
		{
            genericCleanup(stats);
			if (cone) Destroy(cone.gameObject);
		}
	}

    private void genericStatChange(AbilityData abilData, PlayerCharacterStats stats)
    {
        GetComponent<TurnManager>().AutoEndTurnCheck();
        abilData.Currcooldown = abilData.Maxcooldown;
        stats.Actionsleft--;
        GetComponent<TurnManager>().AutoEndTurnCheck();
    }

    private void genericCleanup(PlayerCharacterStats stats)
    {
        stats.CheckCharacterCannotMove();
        GetComponent<PlayerMovementManager>().enabled = true;
        if (stats.Actionsleft == 0 && stats.hasMoved)
        {
            GetComponent<PlayerMovementManager>().Deselect();
        }
        else if (stats.Actionsleft == 0 && !stats.hasMoved)
        {
            GetComponent<PlayerMovementManager>().Select(stats);
            DisableButtons(stats);
        }
        else
            GetComponent<PlayerMovementManager>().Select(stats);
    }
}
