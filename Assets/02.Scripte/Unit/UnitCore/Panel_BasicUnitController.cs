using System.Collections;
using UnitGaugeSample;
using UnitSample;
using UnityEngine;
using static Panel_BasicUnitAI;

public class Panel_BasicUnitController : MonoBehaviour
{

    /// <summary> (prefab) 투사체 프리팹입니다. </summary>
    public GameObject _projectile;
    /// <summary> (prefab) 피격효과 프리팹입니다. </summary>
    public GameObject _attackedEffect;

    public Graphic_Model _model;

    public float _animationTime_attackStart = 1f;
    public float _animationTime_attackEnd = 1f;
    public float _animationTime_attacked = 1f;

    public Global_UnitManager.eUnitKind _unitKind = Global_UnitManager.eUnitKind.None;
    // 고려
    public Data_NormalUnit.UnitState unitState;
    private Panel_BasicUnitAI AI = null;

    public UnitGauge _HPbar = null;

    [HideInInspector]
    public UnitPool _poolUnit = null;

    protected bool InitCheck = false;

    private void Start()
    {
        StartCoroutine(DelayStart());
    }

    protected virtual IEnumerator DelayStart()
    {
        yield return null;
        _poolUnit = new UnitPool();
        GameManager._instance._unitManager._poolUnit.Add(_poolUnit);
        _poolUnit.SetPoolNumber();
        _poolUnit._thisObject = gameObject;

        _poolUnit.OnActived = OnPoolEnable;
        _poolUnit.OnUnActived = OnPoolDisable;
        if (_unitKind == Global_UnitManager.eUnitKind.None) // 유닛풀에서 생성된 유닛 
        {
            gameObject.SetActive(false);

        }
        else  // 에디터상에 배치되어있는 유닛
        {

            _poolUnit._isAssign = true;

            GameManager._instance._unitManager.PoolCheckUnit();

            _poolUnit.OnActived(Global_UnitManager.eUnitKind.None, 0, false);
        }
    }

    protected virtual void AIInit()
    {
        switch (unitState._AIType)
        {
            case Data_BasicUnitData.eAIType.Normal:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            case Data_BasicUnitData.eAIType.UnArmed:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            case Data_BasicUnitData.eAIType.Passive:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            case Data_BasicUnitData.eAIType.Active:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            case Data_BasicUnitData.eAIType.Aggressive:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            case Data_BasicUnitData.eAIType.Defensive:
                AI = gameObject.AddComponent<Panel_BasicUnitAI>();
                break;
            default:
                break;
        }

        AI.animator = _model._animator;


        AI.InitAI();

        UpdateAIVariable();

        AI.HPBarMove();
    }

    private void InitHPbar()
    {
        _HPbar = GameManager._instance._unitManager.PoolSetUnitGauge();

        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);

        _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }

    /// <summary>
    /// (예외대응) 
    /// <br> 에디터에 배치된 유닛들이 게임매니저의 인스턴스 생성과정보다 빠를 경우 이 함수가 실행됩니다. </br>
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator StandbyGameManager()
    {
        while (GameManager._instance == null || AI == null)
        {
            yield return new WaitForSeconds(2.0f);
        }
        GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.DelegateSettingError);
        GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
        AI.AutoScheduler(ePattern.Continue);
    }

    public virtual void OnPoolEnable(Global_UnitManager.eUnitKind unitKind, int teamNumber, bool isWave)
    {
        StartCoroutine(DelayEnable(unitKind, teamNumber, isWave));
    }

    public virtual IEnumerator DelayEnable(Global_UnitManager.eUnitKind unitKind, int teamNumber, bool isWave)
    {
        yield return null;

        if (unitKind != Global_UnitManager.eUnitKind.None) _unitKind = unitKind;

        GameUnitInit();

        AIInit();

        unitState._isWaveUnit = isWave;
        if (teamNumber != 0) unitState._TeamNumber = teamNumber;

        GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
        AI.AutoScheduler(ePattern.Continue);

        AI._cognitiveRange.enabled = true;

        if (_HPbar._bar != null) _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }

    public virtual void OnPoolDisable(Global_UnitManager.eUnitKind unitKind, int teamNumber, bool isWave)
    {
        if (_HPbar != null) _HPbar._isAssign = false;
        GameManager._instance._unitManager.OnStateChangeUnit -= AI.OnChangingEvent;

        if (unitState._isWaveUnit == true) GameManager._instance._waveManager.CheckWaveUnitDieCount();

        AI._cognitiveRange.enabled = false;
        gameObject.SetActive(false);
    }

    public virtual bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }

    public virtual bool EventDamage(Panel_BasicUnitController opponent, bool IgnoreDefend, bool IgnoreProtect)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState, IgnoreDefend, IgnoreProtect);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }


    public virtual IEnumerator OnEffectAttacked()
    {

        yield break;
    }

    public virtual IEnumerator OnEffectProjectile()
    {
        yield return new WaitForSeconds(_animationTime_attackStart);
        yield break;
    }


    protected virtual void GameUnitInit()
    {
        unitState = new Data_NormalUnit.UnitState((int)_unitKind);

        InitHPbar();

        _model.SetMesh(_unitKind);
    }

    protected virtual void UpdateAIVariable()
    {
        AI._cognitiveRange.radius = unitState._cognitveRange;
        AI.delayAttackTiming = new WaitForSeconds(_animationTime_attackStart);
        AI.delayAttackEnd = new WaitForSeconds(_animationTime_attackEnd);
    }
}
