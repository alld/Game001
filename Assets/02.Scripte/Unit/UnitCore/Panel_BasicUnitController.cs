using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;
using static Panel_BasicUnitAI;
using UnitSample;

public class Panel_BasicUnitController : MonoBehaviour
{
    public enum eUnitKind
    {
        None,
        A,
        B,
        C
    }

    /// <summary> (prefab) 투사체 프리팹입니다. </summary>
    public GameObject _projectile;
    /// <summary> (prefab) 피격효과 프리팹입니다. </summary>
    public GameObject _attackedEffect;

    public Graphic_Model _model;

    public float _animationTime_attackStart = 1f;
    public float _animationTime_attackEnd = 1f;
    public float _animationTime_attacked = 1f;

    public eUnitKind unitKind = eUnitKind.None;
    // 고려
    public Data_NormalUnit.UnitState unitState;
    private Panel_BasicUnitAI AI = null;

    public UnitGauge _HPbar = null;

    [HideInInspector]
    public UnitPool _poolUnit = null;

    private bool InitCheck = false;

    private void Start()
    {
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        yield return null;
        _poolUnit = new UnitPool();
        GameManager._instance._unitManager._poolUnit.Add(_poolUnit);
        _poolUnit.SetPoolNumber();
        _poolUnit._thisObject = gameObject;

        _poolUnit.OnActived = OnPoolEnable;
        _poolUnit.OnUnActived = OnPoolDisable;
        if (unitKind == eUnitKind.None) // 유닛풀에서 생성된 유닛 
        {
            gameObject.SetActive(false);

        }
        else  // 에디터상에 배치되어있는 유닛
        {

            _poolUnit._isAssign = true;

            GameManager._instance._unitManager.PoolCheckUnit();

            _poolUnit.OnActived();
        }

        Init();

        InitCheck = true;
    }

    private void Init()
    {
        AI = GetComponent<Panel_BasicUnitAI>();

        AI.InitAI();
    }

    private void InitHPbar()
    {
        _HPbar = GameManager._instance._unitManager.PoolSetUnitGauge();

        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);

        _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;

        AI.HPBarMove();
    }

    /// <summary>
    /// (예외대응) 
    /// <br> 에디터에 배치된 유닛들이 게임매니저의 인스턴스 생성과정보다 빠를 경우 이 함수가 실행됩니다. </br>
    /// </summary>
    /// <returns></returns>
    protected IEnumerator StandbyGameManager()
    {
        while (GameManager._instance == null || AI == null)
        {
            yield return new WaitForSeconds(2.0f);
        }
        GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.DelegateSettingError);
        GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
        AI.AutoScheduler(ePattern.Continue);
    }

    public void OnPoolEnable()
    {
        StartCoroutine(DelayEnable());
    }

    public IEnumerator DelayEnable()
    {
        while (InitCheck == false)
        {
            yield return null;
        }

        GameUnitInit();

        GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
        AI.AutoScheduler(ePattern.Continue);

        AI._cognitiveRange.enabled = true;

        if (_HPbar._bar != null) _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }

    public void OnPoolDisable()
    {
        if (_HPbar != null) _HPbar._isAssign = false;
        GameManager._instance._unitManager.OnStateChangeUnit -= AI.OnChangingEvent;

        AI._cognitiveRange.enabled = false;
        gameObject.SetActive(false);
    }

    public bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }

    public bool EventDamage(Panel_BasicUnitController opponent, bool IgnoreDefend, bool IgnoreProtect)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState, IgnoreDefend, IgnoreProtect);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }


    IEnumerator OnEffectAttacked()
    {

        yield break;
    }

    IEnumerator OnEffectProjectile()
    {
        yield return new WaitForSeconds(_animationTime_attackStart);
        yield break;
    }


    protected void GameUnitInit()
    {
        unitState = new Data_NormalUnit.UnitState((int)unitKind);

        InitHPbar();

        _model.SetMesh(unitKind);

        UpdateAIVariable();
    }

    protected void UpdateAIVariable()
    {
        AI._cognitiveRange.radius = unitState._cognitveRange;
        AI.delayAttackTiming = new WaitForSeconds(_animationTime_attackStart);
        AI.delayAttackEnd = new WaitForSeconds(_animationTime_attackEnd);
    }
}
