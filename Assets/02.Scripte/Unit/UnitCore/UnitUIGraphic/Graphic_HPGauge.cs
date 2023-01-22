using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitGaugeSample;

public class Graphic_HPGauge : MonoBehaviour
{
    public UnitGauge _unitGauge = null;


    private void Start()
    {
        GameManager._instance._unitManager._poolHPbar.Add(_unitGauge);
        GameManager._instance._unitManager._poolHPBar_count.Add(GameManager._instance._unitManager._poolHPbar.Count - 1);

        _unitGauge._isActive = false;

    }
}
