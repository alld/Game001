using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;
using static Panel_BasicUnitAI;

public class Panel_BasicUnitController : MonoBehaviour
{
    public enum eUnitKind
    {
        A,
        B,
        C
    }

    /// <summary> (prefab) ����ü �������Դϴ�. </summary>
    public GameObject _projectile;
    /// <summary> (prefab) �ǰ�ȿ�� �������Դϴ�. </summary>
    public GameObject _attackedEffect;

    public float _animationTime_attackStart = 1f;
    public float _animationTime_attackEnd = 1f;
    public float _animationTime_attacked = 1f;

    public eUnitKind unitKind = eUnitKind.A;
    // ���
    public Data_NormalUnit.UnitState unitState;
    private Panel_BasicUnitAI AI = null;

    public UnitGauge _HPbar = null;

    private void Awake()
    {
        AI = GetComponent<Panel_BasicUnitAI>();
        unitState = new Data_NormalUnit.UnitState((int)unitKind);

        AIInit();

        StartCoroutine(InitHPbar());
    }

    private IEnumerator InitHPbar()
    {
        yield return null;

        _HPbar = GameManager._instance._unitManager.PoolSetUnitGauge();

        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);

        AI.HPBarMove();
    }

    private void OnEnable()
    {
        if (GameManager._instance != null)
        {
            GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
            AI.AutoScheduler(ePattern.Continue);
        }
        else StartCoroutine(StandbyGameManager());
    }

    /// <summary>
    /// (���ܴ���) 
    /// <br> �����Ϳ� ��ġ�� ���ֵ��� ���ӸŴ����� �ν��Ͻ� ������������ ���� ��� �� �Լ��� ����˴ϴ�. </br>
    /// </summary>
    /// <returns></returns>
    protected IEnumerator StandbyGameManager()
    {
        while (GameManager._instance == null)
        {
            yield return new WaitForSeconds(2.0f);
        }
        GameManager._instance._logManager.InputErrorLog(EnumError.ErrorKind.DelegateSettingError);
        GameManager._instance._unitManager.OnStateChangeUnit += AI.OnChangingEvent;
        AI.AutoScheduler(ePattern.Continue);
    }

    private void OnDisable()
    {
        GameManager._instance._unitManager.OnStateChangeUnit -= AI.OnChangingEvent;
    }




    public bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        if(check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
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


    protected void AIInit()
    {
        AI.InitAI();

        UpdateAIVariable();
    }

    protected void UpdateAIVariable()
    {
        AI._cognitiveRange.radius = unitState._cognitveRange;
        AI.delayAttackTiming = new WaitForSeconds(_animationTime_attackStart);
        AI.delayAttackEnd = new WaitForSeconds(_animationTime_attackEnd);
    }
}
