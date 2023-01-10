using EnumError;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Global_TeamSetting : MonoBehaviour
{
    public enum PlayerList
    {
        Player = 0,
        PlayerUnit,
        Enemy
    }

    private static int playerCount = 0;
    public int _playerNumer { get { return playerCount; } }

    public List<PlayerNumber> _players = new List<PlayerNumber>();


    [System.Serializable]
    public struct PlayerNumber
    {
        public string _name;
        public PlayerList _number;

        public List<bool> _isTeam;
        public List<bool> _isEnemy;
    }

    public void Init()
    {
        playerCount = _players.Count;

        CheckPlayerTeam();
    }

    public bool GetTeamStatus(int self, int other)
    {
        return _players[self]._isTeam[other];
    }

    public bool GetEnemyStatus(int self, int other)
    {
        return _players[self]._isEnemy[other];
    }

    /// <summary>
    /// (기능) 
    /// <br>플레이어 팀이 제대로 설정되어있는지 확인합니다. </br>
    /// <br>잘못된 값이 설정되어 있다면 기본값으로 조정됩니다. </br>
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerTeam()
    {
        bool tryChange = false;
        for (int i = 0; i < _players.Count; i++)
        {            
            if ((_players[i]._isTeam.Count != _players[i]._isEnemy.Count) || (_players[i]._isTeam.Count == 0)) // 팀설정된 플레이어수가 일치하지않는 경우
            {
                return RestoreTeamSetting();
            }
            for (int j = 0; j < _players.Count; j++)
            {
                if (_players[i]._isTeam[j] && _players[i]._isEnemy[j]) // 적과 팀설정을 동일하게 설정한 경우 모두 false로 셋팅
                {
                    GameManager._instance._logManager.InputErrorLog(ErrorKind.TeamSettingError);
                    tryChange = true;
                    _players[i]._isTeam[j] = false;
                    _players[i]._isEnemy[j] = false;
                }
            }
            if (_players[i]._isTeam[i] != true || _players[i]._isEnemy[i]) // 플레이어 자신을 적 또는 팀설정이 안되어있는 경우
            {
                _players[i]._isTeam[i] = true;
                _players[i]._isEnemy[i] = false;
                tryChange = true;
                GameManager._instance._logManager.InputErrorLog(ErrorKind.TeamSettingError);
            }
        }
        return tryChange;
    }

    /// <summary>
    /// (예외처리 대응) 
    /// <br>플레이어 별로 팀설정 수가 맞지 않을 경우 디폴트값으로 설정합니다. </br>
    /// <br>해당 함수가 실행될 경우 제대로된 작동이 아니기때문에 에러로그가 수집됩니다.</br>
    /// </summary>
    /// <returns></returns>
    private bool RestoreTeamSetting()
    {
        GameManager._instance._logManager.InputErrorLog(ErrorKind.TeamMisMatchError);
        if (playerCount == 0) return false; // 무한 루프 방지
        int checkCount = 0;
        foreach (var player in _players)
        {
            checkCount = player._isTeam.Count - playerCount;
            if (checkCount < 0)
            {
                for (int i = 0; i < Math.Abs(checkCount); i++)
                {
                    player._isTeam.Add(false);
                }
            }
            else if (checkCount > 0)
            {
                for (int i = 0; i < checkCount; i++)
                {
                    player._isTeam.RemoveAt(player._isTeam.Count - 1);
                }
            }

            checkCount = player._isEnemy.Count - playerCount;
            if (checkCount < 0)
            {
                for (int i = 0; i < Math.Abs(checkCount); i++)
                {
                    player._isEnemy.Add(false);
                }
            }
            else if (checkCount > 0)
            {
                for (int i = 0; i < checkCount; i++)
                {
                    player._isEnemy.RemoveAt(player._isEnemy.Count - 1);
                }
            }
        }
        return CheckPlayerTeam();
    }
}
