using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPortraitChanger : MonoBehaviour {
    RawImage raw;
    Slider HPbar;
    Text Name { get; set; }
    [SerializeField]
    GameObject HPTextOBJ;
    Text HPText;
    private EnemyStats enemy;
    private bool active = true;

    // Use this for initialization
    void Start()
    {
        raw = GetComponent<RawImage>();
        Name = GetComponentInChildren<Text>();
        HPbar = GetComponentInChildren<Slider>();
        HPText = HPTextOBJ.GetComponent<Text>();

    }

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Enemy")) && hit.transform.GetComponent<Renderer>().enabled == true)
        {
            var obj = hit.transform;
            while (obj.parent && LayerMask.LayerToName(hit.transform.gameObject.layer) == "Enemy")
            {
                obj = obj.parent;
            }
            enemy = obj.GetComponentInParent<EnemyStats>();
            changeSelected();

            if (!active)
            {
                raw.enabled = true;
                Name.enabled = true;
                HPbar.enabled = true;
                HPText.enabled = true;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
                active = true;
            }
        }
        else
        {
            
            if (active)
            {
                raw.enabled = false;
                Name.enabled = false;
                HPbar.enabled = false;
                HPText.enabled = false;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }
                active = false;
            }
        }
    }

    private void changeSelected()
    {
        if (enemy)
        {
            raw.texture = enemy.Portrait;
            Name.text = enemy.Name;
            HPbar.maxValue = enemy.MaxHP;
            HPbar.value = enemy.CurrHP;
            HPText.text = "HP: " + enemy.CurrHP;
        }
    }

}
