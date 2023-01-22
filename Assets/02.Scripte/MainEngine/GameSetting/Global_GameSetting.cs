using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ���� �ʿ� : �����α� ���� ȯ�漳�� �߰� ����
// Ŭ����, �ش� ����â ���°� ����;

public class Global_GameSetting : MonoBehaviour
{
    //GS == GameSetting
    public enum eGS_Resolution
    {
        defalut = 0,
        n1024x760,
        n1024x768,
        n1152x720,
        n1152x768,
        n1280x720,
        n1280x768,
        n1280x960,
        n1280x1024,
        n1366x768,
        n1400x1050,
        n1440x900,
        n1440x960,
        n1440x1080,
        n1600x900,
        n1600x1024,
        n1680x1050,
        n1920x800,
        n1920x1080,
        n1920x1200
    };
    public enum eGS_Language
    {
        Korean, English, Japanese
    }

    [Header("���Ӽ��� ����")]
    public string dataPath;

    [Header("�׷���")]
    public eGS_Resolution _GS_Resolution; // �ػ�
    private int GS_width;
    private int GS_height;
    public bool _GS_Window; // ������
    [Range(0, 100)] public int _GS_Brightness; // ���

    [Header("����")]
    [Range(0, 100)] public int _GS_Master;
    [Range(0, 100)] public int _GS_Effects;
    [Range(0, 100)] public int _GS_Interface;
    [Range(0, 100)] public int _GS_Music;


    public bool _GS_MuteSound;
    public bool _GS_MuteBackgoundMusic;

    [Header("���Ӽ���")]

    public eGS_Language _GS_Language;
    public bool _GS_UnitHPBar;


    public void Init()
    {
        InitVariable();

        LoadGameSetting();
    }

    private void InitVariable()
    {
        _GS_Resolution = eGS_Resolution.defalut; // �ػ�
        _GS_Window = false; // ������
        _GS_Brightness = 50; // ���


        _GS_Master = 50;
        _GS_Effects = 100;
        _GS_Interface = 100;
        _GS_Music = 100;


        _GS_MuteSound = false;
        _GS_MuteBackgoundMusic = false;


        _GS_Language = eGS_Language.Korean;
        _GS_UnitHPBar = true;
    }

    public void SaveGameSetting()
    {
        FileStream fileStream = new FileStream(dataPath, FileMode.Create, FileAccess.Write);
        StreamWriter writer = new StreamWriter(fileStream);
        writer.WriteLineAsync("_�ΰ��ӿ��� ȯ�漳���� �����ϴ°��� �����մϴ�.");
        writer.WriteLineAsync("_�߸��� ������ ��� �⺻������ �ݿ��˴ϴ�.\n");
        writer.WriteLineAsync("_�׷���(Graphic)\n");
        writer.WriteLineAsync("_�ػ�(resolution) : #" + (int)_GS_Resolution + "#");
        writer.WriteLineAsync("_â���(window) : #" + _GS_Window + "#");
        writer.WriteLineAsync("_���(brightness) : #" + _GS_Brightness + "#");
        writer.WriteLineAsync("\n_����(Sound)\n");
        writer.WriteLineAsync("_������(master) : #" + _GS_Master + "#");
        writer.WriteLineAsync("_ȿ����(effect) : #" + _GS_Effects + "#");
        writer.WriteLineAsync("_�������̽�(interface) : #" + _GS_Interface + "#");
        writer.WriteLineAsync("_�����(music) : #" + _GS_Music + "#");
        writer.WriteLineAsync("\n_�Ҹ� ���Ұ�(soundMute) : #" + _GS_MuteSound + "#");
        writer.WriteLineAsync("_����� ���Ұ�(musicMute) : #" + _GS_MuteBackgoundMusic + "#");
        writer.WriteLineAsync("\n_���Ӽ���(game)\n");
        writer.WriteLineAsync("_���(Language) : #" + (int)_GS_Language + "#");
        writer.WriteLineAsync("_���� ü�¹�(HP Bar) : #" + _GS_UnitHPBar + "#");

        writer.Close();
    }

    [SerializeField]

    public void LoadGameSetting()
    {
        if (File.Exists(dataPath) == true) // ȯ�漳�� ������ �ִ°��
        {
            string temp_textContent;
            string[] temp_textarray;
            List<string> GS_LoadData = new List<string>();

            FileStream fileStream = new FileStream(dataPath, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(fileStream);
            temp_textContent = reader.ReadToEnd();
            temp_textContent = temp_textContent.Replace("\n", string.Empty);
            temp_textContent = temp_textContent.Replace("\r", string.Empty);
            temp_textarray = temp_textContent.Split("#");

            for (int i = 0; i < temp_textarray.Length - 1; i++)
            {
                if (temp_textarray[i][0] != '_') GS_LoadData.Add(temp_textarray[i]);
            }

            try
            {
                int temp_enumint = 0;

                if(int.TryParse(GS_LoadData[0], out temp_enumint)) _GS_Resolution = (eGS_Resolution)temp_enumint;
                bool.TryParse(GS_LoadData[1], out _GS_Window);
                int.TryParse(GS_LoadData[2], out _GS_Brightness);

                int.TryParse(GS_LoadData[3], out _GS_Master);
                int.TryParse(GS_LoadData[4], out _GS_Effects);
                int.TryParse(GS_LoadData[5], out _GS_Interface);
                int.TryParse(GS_LoadData[6], out _GS_Music);

                bool.TryParse(GS_LoadData[7], out _GS_MuteSound);
                bool.TryParse(GS_LoadData[8], out _GS_MuteBackgoundMusic);

                if (int.TryParse(GS_LoadData[9], out temp_enumint)) _GS_Language = (eGS_Language)temp_enumint;
                bool.TryParse(GS_LoadData[10], out _GS_UnitHPBar);
            }
            catch (System.Exception)
            {
                InitVariable();
                throw;
            }



            reader.Close();

        }
        else // ȯ�漳�� ������ ���°��
        {
            SaveGameSetting();
        }
    }

    public void SetGameSetting()
    {
        switch (_GS_Resolution)
        {
            case eGS_Resolution.defalut:
                GS_width = Screen.width;
                GS_height = Screen.height;
                break;
            case eGS_Resolution.n1024x760:
                GS_width = 1024;
                GS_height = 760;
                break;
            case eGS_Resolution.n1024x768:
                GS_width = 1024;
                GS_height = 768;
                break;
            case eGS_Resolution.n1152x720:
                GS_width = 1152;
                GS_height = 720;
                break;
            case eGS_Resolution.n1152x768:
                GS_width = 1152;
                GS_height = 768;
                break;
            case eGS_Resolution.n1280x720:
                GS_width = 1280;
                GS_height = 720;
                break;
            case eGS_Resolution.n1280x768:
                GS_width = 1280;
                GS_height = 768;
                break;
            case eGS_Resolution.n1280x960:
                GS_width = 1280;
                GS_height = 960;
                break;
            case eGS_Resolution.n1280x1024:
                GS_width = 1280;
                GS_height = 1024;
                break;
            case eGS_Resolution.n1366x768:
                GS_width = 1366;
                GS_height = 768;
                break;
            case eGS_Resolution.n1400x1050:
                GS_width = 1400;
                GS_height = 1050;
                break;
            case eGS_Resolution.n1440x900:
                GS_width = 1440;
                GS_height = 900;
                break;
            case eGS_Resolution.n1440x960:
                GS_width = 1440;
                GS_height = 960;
                break;
            case eGS_Resolution.n1440x1080:
                GS_width = 1440;
                GS_height = 1080;
                break;
            case eGS_Resolution.n1600x900:
                GS_width = 1600;
                GS_height = 900;
                break;
            case eGS_Resolution.n1600x1024:
                GS_width = 1600;
                GS_height = 1024;
                break;
            case eGS_Resolution.n1680x1050:
                GS_width = 1680;
                GS_height = 1050;
                break;
            case eGS_Resolution.n1920x800:
                GS_width = 1920;
                GS_height = 800;
                break;
            case eGS_Resolution.n1920x1080:
                GS_width = 1920;
                GS_height = 1080;
                break;
            case eGS_Resolution.n1920x1200:
                GS_width = 1920;
                GS_height = 1200;
                break;
        }
        Screen.SetResolution(GS_width, GS_height, !_GS_Window);

        Global_TextManager.SetLangauge(_GS_Language);
    }

    public void onClickTest()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }
}
