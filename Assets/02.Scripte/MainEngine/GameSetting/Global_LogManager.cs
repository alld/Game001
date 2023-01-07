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
                SB.Append(": ������ �������� �߸��� ������ �����մϴ�.");
                break;
                case ErrorKind.TeamMisMatchError:
                SB.Append(": �÷��̾��� ���� ���� ���� �˸°� �����Ǿ����� �ʽ��ϴ�.");
                break;
        }
        log.Add(SB.ToString());
        if (putLog == true)
        {
            Debug.Log(log[(int)error]);
        }
    }
}
