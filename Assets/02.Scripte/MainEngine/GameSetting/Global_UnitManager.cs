using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;
using UnitSample;
using UnitFlowTextSample;

public class Global_UnitManager : MonoBehaviour
{
    public enum eUnitKind
    {
        None,
        A,
        B,
        C
    }


    [Header("오브젝트 풀")]
    public GameObject canvas;
    private const int AddNumber = 5;

    public delegate void DeleChangeUnit(int objectID);
    public DeleChangeUnit OnStateChangeUnit;

    public void Init()
    {
        canvas = GameObject.Find("HPbarGroup");

        PoolAddUnitGauge();

        OnStateChangeUnit = null;
    }

    private void OnDestroy()
    {
        OnStateChangeUnit = null;
    }

    public int CreateUnit(eUnitKind unitKind, Vector3 CreatePoint,int teamNumber,bool isWave = false)
    {
        var NewUnit = GetPoolUnit();
        NewUnit._thisObject.transform.position = CreatePoint;
        //NewUnit.GetComponent<Data_BasicUnitData>();

        NewUnit.OnActived(unitKind, teamNumber, isWave);
        return NewUnit.poolNumber;
    }


    #region HP바 UI풀
    [Header("UI풀 - HPbar")]
    public GameObject prefab_HPBar;
    public List<int> _poolHPBar_count = new List<int>();
    [HideInInspector]
    public List<UnitGauge> _poolHPbar = new List<UnitGauge>();

    public UnitGauge PoolSetUnitGauge()
    {
        int temp_number;
        if(_poolHPBar_count.Count <= 1)
        {
            PoolAddUnitGauge();
        }

        temp_number = _poolHPBar_count[0];
        _poolHPbar[temp_number]._isAssign = true;


        return _poolHPbar[temp_number];
    }

    public void PoolRemoveUnitGauge(int poolnumber)
    {
        _poolHPBar_count.Add(poolnumber);
    }

    public void PoolAllRemoveUnitGauge()
    {
        _poolHPBar_count.Clear();
        for (int i = 0; i < _poolHPbar.Count; i++)
        {
            _poolHPbar[i]._isAssign = false;
        }
    }

    private void PoolAddUnitGauge()
    {
        for (int i = 0; i < AddNumber; i++)
        {
            Instantiate(prefab_HPBar, canvas.transform);
        }
    }
    #endregion
    #region HP바 UI풀
    [Header("UI풀 - 유동텍스트")]
    public GameObject prefab_FlowText;
    public List<int> _poolFlowText_count = new List<int>();
    [HideInInspector]
    public List<FlowText> _poolFlowText = new List<FlowText>();

    public FlowText PoolSetFlowText()
    {
        int temp_number;
        if (_poolFlowText_count.Count <= 1)
        {
            PoolAddFlowText();
        }

        temp_number = _poolFlowText_count[0];
        _poolFlowText[temp_number]._isAssign = true;


        return _poolFlowText[temp_number];
    }

    public void PoolRemoveFlowText(int poolnumber)
    {
        _poolFlowText_count.Add(poolnumber);
    }

    public void PoolAllRemoveFlowText()
    {
        _poolFlowText_count.Clear();
        for (int i = 0; i < _poolFlowText.Count; i++)
        {
            _poolFlowText[i]._isAssign = false;
        }
    }

    private void PoolAddFlowText()
    {
        for (int i = 0; i < AddNumber; i++)
        {
            Instantiate(prefab_FlowText, canvas.transform);
        }
    }
    #endregion

    #region 유닛 풀
    [Header("Unit 풀")]
    public GameObject prefab_Unit;
    public Mesh[] _meshList;
    public Material[] _materialList;
    public List<int> _poolUnit_count = new List<int>();
    public List<UnitPool> _poolUnit = new List<UnitPool>();


    public void PoolRemoveUnit(int poolnumber)
    {
        _poolUnit[poolnumber].OnUnActived(eUnitKind.None, 0, false);
        _poolUnit[poolnumber]._isAssign = false;
        _poolHPBar_count.Add(poolnumber);
    }

    public void PoolAllRemoveUnit()
    {
        _poolUnit_count.Clear();
        for (int i = 0; i < _poolUnit.Count -1; i++)
        {
            _poolUnit[i].OnUnActived(eUnitKind.None, 0, false);
            _poolUnit[i]._isAssign = false;
            _poolHPBar_count.Add(i);
        }
    }

    /// <summary>
    /// (풀:기능) 풀 공간에 할당되지않은 유닛을 불러옵니다. 
    /// <br> 해당기능이 호출되면 해당 유닛의 값이 변동되니 할당이 필요한 경우에만 호출해야합니다.</br>
    /// </summary>
    /// <returns></returns>
    public UnitPool GetPoolUnit()
    {
        PoolCheckUnit();

        int temp_number = _poolUnit_count[0];
        _poolUnit[temp_number]._isAssign = true;


        return _poolUnit[temp_number];
    }

    /// <summary>
    /// (풀:기능) 여유 유닛이 있는지 체크하여 부족할 경우 풀의 공간을 확보합니다.  
    /// </summary>
    public void PoolCheckUnit()
    {
        if (_poolUnit_count.Count <= 3)
        {
            for (int i = 0; i < AddNumber; i++)
            {
                Instantiate(prefab_Unit, transform);
            }
        }
    }

    #endregion

}

namespace UnitSample
{
    public class UnitPool
    {
        public GameObject _thisObject;

        public delegate void DeleUnitActive(Global_UnitManager.eUnitKind unitKind,int teamNumber, bool isWave);
        public DeleUnitActive OnActived;
        public DeleUnitActive OnUnActived;
        
        public int poolNumber;
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
                if(value == true)
                {
                    GameManager._instance._unitManager._poolUnit_count.Remove(poolNumber);
                }
                else
                {
                    if (GameManager._instance._unitManager._poolUnit_count.Exists(x => x == poolNumber) == false) 
                    {
                        GameManager._instance._unitManager._poolUnit_count.Add(poolNumber);
                        _thisObject.transform.SetParent(GameManager._instance._unitManager.transform);
                    }
                }
            } 
        }

        public void SetPoolNumber()
        {
            GameManager._instance._unitManager._poolUnit_count.Add(GameManager._instance._unitManager._poolUnit.Count - 1);
            poolNumber = GameManager._instance._unitManager._poolUnit.Count - 1;
        }
    }
}


namespace UnitGaugeSample
{
    using UnityEngine.UI;

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
                    GameManager._instance._unitManager._poolHPBar_count.RemoveAt(0);
                }
                else
                {
                    GameManager._instance._unitManager.PoolRemoveUnitGauge(poolNumber);
                    _bar.transform.SetParent(GameManager._instance._unitManager.transform);
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

namespace UnitFlowTextSample
{
    using UnityEngine.UI;
    using TMPro;

    [System.Serializable]
    public class FlowText
    {
        public GameObject _flowText;
        public TMP_Text _text;
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
                    GameManager._instance._unitManager._poolHPBar_count.RemoveAt(0);
                }
                else
                {
                    GameManager._instance._unitManager.PoolRemoveUnitGauge(poolNumber);
                    _flowText.transform.SetParent(GameManager._instance._unitManager.transform);
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
                if (value == true)
                {
                    //_frame.color = Color.white;
                    //_gauge.color = Color.white;
                }
                else
                {
                    //_frame.color = Color.clear;
                    //_gauge.color = Color.clear;
                }
            }
        }

        public void UpdateGauage(float maxHP, float HP)
        {
            //_gauge.fillAmount = HP / maxHP;
        }
    }
}