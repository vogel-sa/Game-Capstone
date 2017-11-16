using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour {

    [SerializeField]
    private int _health = 10;
    public int Health
    {
        get
        {
            return _health;
        }
        private set
        {
            _health = value;
        }
    }
    public void TakeDamage(int dmg)
    {
        Health -= dmg;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
        GetComponent<SingleNodeBlocker>().Unblock();
        PathManager.Instance.enemies.Remove(GetComponent<SingleNodeBlocker>());
    }
}
