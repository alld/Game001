using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumError;
using System.Text;

namespace EnumError
{
    public enum ErrorKind
    {
        TeamSettingError,
        TeamMisMatchError,
    }
}

public class Global_LogManager : MonoBehaviour
{
    [SerializeField]
    private bool putLog = false;
    [SerializeField]
    private List<string> log = new List<string>();

    private StringBuilder SB = new StringBuilder();

    public void InputErrorLog(ErrorKind error)
    {
        SB.Clear();
        SB.Append(error.ToString());
        switch (error)
        {
            case ErrorKind.TeamSettingError:
                SB.Append(": 팀셋팅 과정에서 잘못된 설정이 존재합니다.");
                break;
                case ErrorKind.TeamMisMatchError:
                SB.Append(": 플레이어의 팀과 적의 수가 알맞게 설정되어있지 않습니다.");
                break;
        }
        log.Add(SB.ToString());
        if (putLog == true)
        {
            Debug.Log(log[(int)error]);
        }
    }
}
