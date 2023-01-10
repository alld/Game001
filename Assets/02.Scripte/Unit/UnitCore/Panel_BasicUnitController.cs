using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Panel_BasicUnitController : MonoBehaviour
{
    // °í·Á
    public Data_NormalUnit.UnitState unitState;
    private Graphic_BasicUnitAI AI = null;

    private void Start()
    {
        AI = GetComponent<Graphic_BasicUnitAI>();
        unitState = new Data_NormalUnit.UnitState(4);

        AIInit();
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
