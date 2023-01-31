using System.Collections;
using UnitSample;
using UnityEngine;
using UnityEngine.InputSystem;

public class Panel_PlayerController : Panel_BasicUnitController
{
    private Graphic_PlayerStat _playerStat = null;
    public LayerMask _rayMask;

    protected override IEnumerator DelayStart()
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

    protected override void AIInit()
    {
        _playerStat = gameObject.AddComponent<Graphic_PlayerStat>();

        _playerStat._animator = _model._animator;

        _playerStat.Init();

        _playerStat.HPBarMove();
    }

    private void InitHPbar()
    {
        _HPbar = GameManager._instance._unitManager.PoolSetUnitGauge();

        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);

        _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }


    public override IEnumerator DelayEnable(Global_UnitManager.eUnitKind unitKind, int teamNumber, bool isWave)
    {
        yield return null;

        if (unitKind != Global_UnitManager.eUnitKind.None) _unitKind = unitKind;

        GameUnitInit();

        AIInit();

        unitState._isWaveUnit = isWave;
        if (teamNumber != 0) unitState._TeamNumber = teamNumber;


        if (_HPbar._bar != null) _HPbar._isActive = GameManager._instance._gameSetting._GS_UnitHPBar;
    }

    public override void OnPoolDisable(Global_UnitManager.eUnitKind unitKind, int teamNumber, bool isWave)
    {
        if (_HPbar != null) _HPbar._isAssign = false;

        gameObject.SetActive(false);
    }

    public override bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }

    public override bool EventDamage(Panel_BasicUnitController opponent, bool IgnoreDefend, bool IgnoreProtect)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState, IgnoreDefend, IgnoreProtect);
        if (check == false) GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        _HPbar.UpdateGauage(unitState.maxHP, unitState._HP);
        return check;
    }


    public override IEnumerator OnEffectAttacked()
    {

        yield break;
    }

    public override IEnumerator OnEffectProjectile()
    {
        yield return new WaitForSeconds(_animationTime_attackStart);
        yield break;
    }


    protected override void GameUnitInit()
    {
        unitState = new Data_NormalUnit.UnitState((int)_unitKind);

        InitHPbar();

        _model.SetMesh(_unitKind);

    }


    public void OnClickMovePoint(InputAction.CallbackContext context)
    {
        if (context.started == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray,out hit,100f, _rayMask))
            {
                _playerStat.OnUnitMove(hit);
            }
        }
    }
}
