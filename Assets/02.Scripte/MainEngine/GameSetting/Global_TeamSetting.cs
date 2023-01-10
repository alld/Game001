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
    /// (���) 
    /// <br>�÷��̾� ���� ����� �����Ǿ��ִ��� Ȯ���մϴ�. </br>
    /// <br>�߸��� ���� �����Ǿ� �ִٸ� �⺻������ �����˴ϴ�. </br>
    /// </summary>
    /// <returns></returns>
    public bool CheckPlayerTeam()
    {
        bool tryChange = false;
        for (int i = 0; i < _players.Count; i++)
        {            
            if ((_players[i]._isTeam.Count != _players[i]._isEnemy.Count) || (_players[i]._isTeam.Count == 0)) // �������� �÷��̾���� ��ġ�����ʴ� ���
            {
                return RestoreTeamSetting();
            }
            for (int j = 0; j < _players.Count; j++)
            {
                if (_players[i]._isTeam[j] && _players[i]._isEnemy[j]) // ���� �������� �����ϰ� ������ ��� ��� false�� ����
                {
                    GameManager._instance._logManager.InputErrorLog(ErrorKind.TeamSettingError);
                    tryChange = true;
                    _players[i]._isTeam[j] = false;
                    _players[i]._isEnemy[j] = false;
                }
            }
            if (_players[i]._isTeam[i] != true || _players[i]._isEnemy[i]) // �÷��̾� �ڽ��� �� �Ǵ� �������� �ȵǾ��ִ� ���
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
    /// (����ó�� ����) 
    /// <br>�÷��̾� ���� ������ ���� ���� ���� ��� ����Ʈ������ �����մϴ�. </br>
    /// <br>�ش� �Լ��� ����� ��� ����ε� �۵��� �ƴϱ⶧���� �����αװ� �����˴ϴ�.</br>
    /// </summary>
    /// <returns></returns>
    private bool RestoreTeamSetting()
    {
        GameManager._instance._logManager.InputErrorLog(ErrorKind.TeamMisMatchError);
        if (playerCount == 0) return false; // ���� ���� ����
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
