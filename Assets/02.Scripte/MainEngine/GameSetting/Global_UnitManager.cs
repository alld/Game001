using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Global_UnitManager : MonoBehaviour
{
    public delegate void DeleChangeUnit(int objectID);
    public DeleChangeUnit OnStateChangeUnit;

    public void Init()
    {
        OnStateChangeUnit = null;
    }
}
