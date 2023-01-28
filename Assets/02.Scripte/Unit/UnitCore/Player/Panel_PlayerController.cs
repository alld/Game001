using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;
using static Panel_BasicUnitAI;
using UnitSample;

public class Panel_PlayerController : Panel_BasicUnitController
{
    private Graphic_PlayerStat _playerStat = null;

    private bool InitCheck = false;

    private new IEnumerator DelayStart()
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

    private new void Init()
    {
        _playerStat = gameObject.AddComponent<Graphic_PlayerStat>();

        _playerStat._animator = _model._animator;

        _playerStat.Init();
    }

    private void InitHPbar()
    {
        _HPbar = GameManager._instance._unitManager.PoolSetUnitGauge();

        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);

        _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;

        _playerStat.HPBarMove();
    }


    public new void OnPoolEnable()
    {
        StartCoroutine(DelayEnable());
    }

    public new IEnumerator DelayEnable()
    {
        while (InitCheck == false)
        {
            yield return null;
        }

        GameUnitInit();


        if (_HPbar._bar != null) _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }

    public new void OnPoolDisable()
    {
        if (_HPbar != null) _HPbar._isAssign = false;

        gameObject.SetActive(false);
    }

    public new bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }

    public new bool EventDamage(Panel_BasicUnitController opponent, bool IgnoreDefend, bool IgnoreProtect)
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


    protected new void GameUnitInit()
    {
        unitState = new Data_NormalUnit.UnitState((int)unitKind);

        InitHPbar();

        _model.SetMesh(unitKind);

        UpdateAIVariable();
    }
}
