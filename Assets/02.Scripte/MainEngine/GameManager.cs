using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager _instance = null;
    public Global_TeamSetting _teamSetting = null;
    public Global_LogManager _logManager = null;

    private void Awake()
    {
        if (_instance == null) { 
            _instance = this; 
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this); }

        GameInit();
    }

    private void GameInit() {
        _teamSetting.CheckPlayerTeam(); // 팀설정이 제대로되어있는지 확인합니다. 
    }
    public static string gameVersion = "0.00";

}
