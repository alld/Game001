using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;
using UnityEngine.UI;

public class Global_UnitManager : MonoBehaviour
{
    public GameObject prefab_HPBar;
    public GameObject canvas;

    public delegate void DeleChangeUnit(int objectID);
    public DeleChangeUnit OnStateChangeUnit;

    public void Init()
    {
        canvas = GameObject.Find("HPbarGroup");

        //PoolAddUnitGauge();

        OnStateChangeUnit = null;
    }

    private void OnDestroy()
    {
        OnStateChangeUnit = null;
    }

    public List<UnitPool> _poolUnit = new List<UnitPool>();
    //[HideInInspector]
    public List<UnitGauge> _poolHPbar = new List<UnitGauge>();
    public List<int> _poolHPBar_count = new List<int>();
    private const int AddNumber = 5;

    public UnitGauge PoolSetUnitGauge()
    {
        int temp_number;
        if(_poolHPBar_count.Count == 1)
        {
            PoolAddUnitGauge();
        }

        temp_number = _poolHPBar_count[0];
        _poolHPBar_count.RemoveAt(0);
        _poolHPbar[temp_number]._isAssign = true;


        return _poolHPbar[temp_number];
    }

    public void PoolRemoveUnitGauge(int poolnumber)
    {
        _poolHPBar_count.Add(poolnumber);
    }

    private void PoolAddUnitGauge()
    {
        for (int i = 0; i < AddNumber; i++)
        {
            Instantiate(prefab_HPBar, canvas.transform);
        }
    }

    public class UnitPool
    {
        GameObject _prefab;
        GameObject _gameObject;

        
    }


}


namespace UnitGaugeSample
{
    [System.Serializable]
    public class UnitGauge
    {
        public GameObject _bar;
        public Image _frame;
        public Image _gauge;
        private int poolNumber;

        private bool isAssign = false;
        public bool _isAssign
        {
            get
            {
                return isAssign;
            }
            set
            {
                isAssign = value;
                if (value == true)
                {
                    poolNumber = GameManager._instance._unitManager._poolHPBar_count[0];
                }
                else
                {
                    GameManager._instance._unitManager.PoolRemoveUnitGauge(poolNumber);
                }
            }
        }

        private bool isActive = false;
        public bool _isActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if(value == true)
                {
                    _frame.color = Color.white;
                    _gauge.color = Color.white;
                }
                else
                {
                    _frame.color = Color.clear;
                    _gauge.color = Color.clear;
                }
            }
        }

        public void UpdateGauage(float maxHP, float HP)
        {
            _gauge.fillAmount = HP / maxHP;
        }
    }
}