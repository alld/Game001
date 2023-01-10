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
    /// (���) 
    /// <br>���߰������� ���� ������ �������� �α� ������� ����ϴ�.</br>
    /// <br>�ش� ��ɵ��� ����ó���� �����κ� ������ �Ǿ������� </br>
    /// <br>����ε� �����Ͱ� �Ҵ�Ǿ����� ���� ���ɼ��� �����ϴ�.</br>
    /// <br>�αװ� �߻��� �������� ���� �̽� ������ �ʿ���մϴ�.</br>
    /// </summary>
    /// <param name="error"></param>
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
            case ErrorKind.AIMissingComponent:
                SB.Append(": AI�� ����Ž�� �������� ������Ʈ ������ �ȵ������� �����մϴ�.");
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
    /// (���) <br>�߻��� �α׵��� ���Ͽ� ��� ��Ͻ�ŵ�ϴ�. </br>
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
