using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel_Intro : MonoBehaviour
{
    private WaitForSeconds delay = new WaitForSeconds(0.5f);
    private void Awake()
    {
        StartCoroutine(RoutineManagerCheck());
    }

    private IEnumerator RoutineManagerCheck()
    {
        while(!GameManager.FinishSetting)
        { 
            yield return delay;
        }
        GameManager._instance.ChangeScene(GameManager.SceneN.Main, false);
    }
}
