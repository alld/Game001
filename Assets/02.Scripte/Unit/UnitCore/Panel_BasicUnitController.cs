using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using static Data_BasicUnitData;

public class Panel_BasicUnitController : MonoBehaviour
{
    // °í·Á
    public Data_NormalUnit.UnitState unitState;
    private Graphic_BasicUnitAI AI = null;

    private void Start()
    {
        AI = GetComponent<Graphic_BasicUnitAI>();
        unitState = new Data_NormalUnit.UnitState(gameObject.GetInstanceID());

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
