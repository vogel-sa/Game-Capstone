using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour, ICharacterStats
{

    [SerializeField]
    string _name;
    public string Name
    {
        get
        {
            return _name;
        }

        set
        {
            _name = value;
        }
    }

    [SerializeField]
    private int _maxHP;
    public int MaxHP
    {
        get
        {
            return _maxHP;
        }

        set
        {
            _maxHP = value;
        }
    }

    private int _currHP; public int CurrHP
    {
        get
        {
            return _currHP;
        }

        set
        {
            if (CurrHP + value > _maxHP)
            {
                _currHP = _maxHP;
            }
            else if (CurrHP - value < 0)
            {
                _currHP = 0;
            }

            _currHP = value;
        }
    }

	[SerializeField]
	private int _atk;
	public int Atk
	{
		get
		{
			return _atk;
		}
		set
		{
			_atk = value;
		}
	}
    /* Represents a scalar damage reduction value   
    A value of 1 would reduce all incoming damage by 1
    Value of 2 reduces all incoming damage by 2, etc.*/
    [SerializeField]
    private int _mitigationValue = 0;
    public int MitigationValue
    {
        get
        {
            return _mitigationValue;
        }

        set
        {
            _mitigationValue = value;
        }
    }

    [SerializeField]
    private int _movementRange;
    public int MovementRange
    {
        get
        {
            return _movementRange;
        }

        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Movement Range cannot be less than 1");
            }
            _movementRange = value;
        }
    }

    [SerializeField]
    private int _actionsleft;
    public int Actionsleft
    {
        get
        {
            return _actionsleft;
        }

        set
        {
            _actionsleft = value;
        }
    }

    [SerializeField]
    Texture2D _portrait;
    public Texture2D Portrait
    {
        get
        {
            return _portrait;
        }

        set
        {
            _portrait = value;
        }
    }

    [SerializeField]
    private float _detectionRadius;
    public float DetectionRadius { get { return _detectionRadius; } }

    void Awake()
    {
        CurrHP = MaxHP;
    }

    public bool IsDead()
    {
        return CurrHP <= 0;
    }

    public void TakeDamage(int dmg)
    {
        CurrHP -= dmg;
        GetComponent<DamageText>().displayText(dmg, 1.1f);

        if (CurrHP <= 0)
        {
            Die();
        }
    }

	private bool flag = false;

	public void swapFlag() {

		if (flag) {
			flag = false;
		}
		else {
			flag = true;
		}
	}

	public bool hitByAbility() {
		return flag;
	}

    private GameObject lastKnownLocation = null;

    void Start()
    {
        lastKnownLocation = Instantiate(Resources.Load<GameObject>("Prefabs/LastKnownLocation"));
        LastKnownLocation lkl = lastKnownLocation.GetComponent<LastKnownLocation>();
        lkl.stats = this;
    }

    private void Die()
    {
        gameObject.SetActive(false);
        GetComponent<SingleNodeBlocker>().Unblock();
		FindObjectOfType<PathManager>().enemies.Remove(GetComponent<SingleNodeBlocker>());
		//eventually change to mark for removal
		FindObjectOfType<TurnManager>().enemyList.Remove (this);
		FindObjectOfType<TurnManager>().CheckGameOver ();
        var pm = FindObjectOfType<PathManager>();
        pm.allies.Remove(GetComponent<SingleNodeBlocker>());
        Destroy(lastKnownLocation);
        
    }
		
}
