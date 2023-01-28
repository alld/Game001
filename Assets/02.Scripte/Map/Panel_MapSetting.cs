using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_MapSetting : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DelayStart());
    }

    private IEnumerator DelayStart()
    {
        while (GameManager._instance._mapManager._completReady == false)
        {
            yield return null;
        }

        GameManager._instance._mapManager.CreateField(transform);

        while (GameManager._instance._mapManager._completSetting == false)
        {
            yield return null;
        }
    }
}
