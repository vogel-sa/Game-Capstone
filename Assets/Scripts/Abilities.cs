using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abilities : MonoBehaviour {

    private static object _lock = new object();
    private static Abilities _instance;
    public static Abilities Instance
    {
        get
        {
            lock (_lock)
            {
                if (!_instance)
                {
                    var inst = FindObjectOfType(typeof(Abilities)) as Abilities;
                    _instance = inst ? inst : new GameObject().AddComponent<Abilities>();
                }
            }
            return _instance;
        }
    }

    public void BasicShootAbility(PlayerCharacterStats stats) { StartCoroutine(_basicShootAbility(stats)); }
    private IEnumerator _basicShootAbility(PlayerCharacterStats stats)
    {
        LineRenderer lineRenderer = null;
        try
        {
            PlayerMovementManager.Instance.SetQuadsEnabled(false);
            PlayerMovementManager.Instance.enabled = false;
            var range = 10f;
            var width = .05f;
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
                    lineRenderer.SetPositions(new Vector3[] { origin, origin - direction.normalized*range });
                    stats.transform.LookAt(new Vector3(direction.x, stats.transform.position.y, direction.z));
                }
                yield return null;
            } while (!Input.GetMouseButtonDown(0));
            lineRenderer.gameObject.SetActive(false);
            // TODO: Animation of ability

            if (Physics.Raycast(stats.transform.position, direction, out hit, range, ~LayerMask.GetMask("Enemy")))
            {
                var hitStats = hit.transform.GetComponent<ICharacterStats>();
                //hitStats.IsDead = true;
            }
            Debug.Log("Bang");


            yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        }
        finally
        {
            PlayerMovementManager.Instance.SetQuadsEnabled(true);
            PlayerMovementManager.Instance.enabled = true;
            if (lineRenderer) Destroy(lineRenderer.gameObject);
        }
    }
}
