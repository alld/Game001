using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnumError;
using System.Text;
using System.IO;
using System;

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

    public string dataPath = "";
    public bool _record = true;
    public bool _overwrite = false;

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
        SB.Append(": date : ");
        SB.Append(DateTime.Now);
        log.Add(SB.ToString());
        if (putLog == true)
        {
            Debug.Log(log[(int)error]);
        }

        RecordLog();
    }

    private void RecordLog()
    {
        if (_record == false) return;

        FileStream fileStream;

        if (_overwrite == false)
        {
            fileStream = new FileStream(dataPath, FileMode.Append, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream);
            writer.WriteLineAsync(log[log.Count - 1]);
            writer.Close();
        }
        else
        {
            fileStream = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fileStream);
            foreach (var text in log)
            {
                writer.WriteLineAsync(text);
            }
            writer.Close();
        };
    }
}
