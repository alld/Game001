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
        AIMissingComponent
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


    /// <summary>
    /// (기능) 
    /// <br>개발과정에서 예측 가능한 설정들을 로그 기록으로 남깁니다.</br>
    /// <br>해당 기능들은 예외처리로 일정부분 대응이 되어있으나 </br>
    /// <br>제대로된 데이터가 할당되어있지 않을 가능성이 높습니다.</br>
    /// <br>로그가 발생한 문제들은 추후 이슈 대응을 필요로합니다.</br>
    /// </summary>
    /// <param name="error"></param>
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
            case ErrorKind.AIMissingComponent:
                SB.Append(": AI의 유닛탐지 과정에서 컴포넌트 설정이 안된유닛이 존재합니다.");
                break;
        }
        SB.Append(": date : ");
        SB.Append(DateTime.Now);
        log.Add(SB.ToString());
        if (putLog == true)
        {
            Debug.Log(log[log.Count-1]);
        }

        RecordLog();
    }

    /// <summary>
    /// (기능) <br>발생한 로그들을 파일에 모두 기록시킵니다. </br>
    /// </summary>
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
