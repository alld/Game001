using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using static Data_BasicUnitData;

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
    private Graphic_BasicUnitAI AI = null;

    private void Start()
    {
        AI = GetComponent<Graphic_BasicUnitAI>();
        unitState = new Data_NormalUnit.UnitState((int)unitKind);

        AIInit();
    }


    public bool EventDamage(Panel_BasicUnitController opponent)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState);
        GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
        return check;
    }

    public bool EventDamage(Panel_BasicUnitController opponent, bool IgnoreDefend, bool IgnoreProtect)
    {
        bool check = unitState.CalculatorDamage(opponent.unitState, IgnoreDefend, IgnoreProtect);
        GameManager._instance._unitManager.OnStateChangeUnit(opponent.gameObject.GetInstanceID());
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
    }
}
