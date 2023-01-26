using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitFlowTextSample;
public class Graphic_FlowText : MonoBehaviour
{
    public FlowText _flowtext = null;

    void Start()
    {
        GameManager._instance._unitManager._poolFlowText.Add(_flowtext);
        GameManager._instance._unitManager._poolFlowText_count.Add(GameManager._instance._unitManager._poolFlowText.Count - 1);

        _flowtext._isActive = false;
    }

}
