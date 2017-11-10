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
        PlayerMovementManager.Instance.SetQuadsEnabled(false);
        var range = 5f;

        GameObject aimLine = Instantiate(Resources.Load<GameObject>("AimLine"));
        aimLine.transform.position = stats.transform.position + Vector3.up / 2;
        // TODO: Control Ability (exit using escape)
        do
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                var v = hit.point;
                aimLine.transform.LookAt(new Vector3(hit.point.x, aimLine.transform.position.y, hit.point.z));
            }
            yield return null;
        } while (!Input.GetMouseButtonDown(0));

        Destroy(aimLine.gameObject);
        // TODO: Animation of ability
        Debug.Log("Bang");
        yield return new WaitForSeconds(.5f);// Change to wait until animation over, possibly wait for enemy reaction (i.e. reaction shot, death anim, etc.);
        PlayerMovementManager.Instance.SetQuadsEnabled(true);
    }
}
