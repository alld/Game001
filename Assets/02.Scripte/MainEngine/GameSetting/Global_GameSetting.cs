using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 수정 필요 : 에러로그 수집 환경설정 추가 검토
// 클릭시, 해당 폴더창 여는거 검토;

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

    [Header("게임설정 저장")]
    public string dataPath;

    [Header("그래픽")]
    public eGS_Resolution _GS_Resolution; // 해상도
    private int GS_width;
    private int GS_height;
    public bool _GS_Window; // 윈도우
    [Range(0, 100)] public int _GS_Brightness; // 밝기

    [Header("음량")]
    [Range(0, 100)] public int _GS_Master;
    [Range(0, 100)] public int _GS_Effects;
    [Range(0, 100)] public int _GS_Interface;
    [Range(0, 100)] public int _GS_Music;


    public bool _GS_MuteSound;
    public bool _GS_MuteBackgoundMusic;

    [Header("게임설정")]

    public eGS_Language _GS_Language;
    public bool _GS_UnitHPBar;


    public void Init()
    {
        InitVariable();

        LoadGameSetting();
    }

    private void InitVariable()
    {
        _GS_Resolution = eGS_Resolution.defalut; // 해상도
        _GS_Window = false; // 윈도우
        _GS_Brightness = 50; // 밝기


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
        writer.WriteLineAsync("_인게임에서 환경설정을 수정하는것을 권장합니다.");
        writer.WriteLineAsync("_잘못된 설정은 모두 기본값으로 반영됩니다.\n");
        writer.WriteLineAsync("_그래픽(Graphic)\n");
        writer.WriteLineAsync("_해상도(resolution) : #" + (int)_GS_Resolution + "#");
        writer.WriteLineAsync("_창모드(window) : #" + _GS_Window + "#");
        writer.WriteLineAsync("_밝기(brightness) : #" + _GS_Brightness + "#");
        writer.WriteLineAsync("\n_음량(Sound)\n");
        writer.WriteLineAsync("_총음량(master) : #" + _GS_Master + "#");
        writer.WriteLineAsync("_효과음(effect) : #" + _GS_Effects + "#");
        writer.WriteLineAsync("_인터페이스(interface) : #" + _GS_Interface + "#");
        writer.WriteLineAsync("_배경음(music) : #" + _GS_Music + "#");
        writer.WriteLineAsync("\n_소리 음소거(soundMute) : #" + _GS_MuteSound + "#");
        writer.WriteLineAsync("_배경음 음소거(musicMute) : #" + _GS_MuteBackgoundMusic + "#");
        writer.WriteLineAsync("\n_게임설정(game)\n");
        writer.WriteLineAsync("_언어(Language) : #" + (int)_GS_Language + "#");
        writer.WriteLineAsync("_유닛 체력바(HP Bar) : #" + _GS_UnitHPBar + "#");

        writer.Close();
    }

    [SerializeField]

    public void LoadGameSetting()
    {
        if (File.Exists(dataPath) == true) // 환경설정 파일이 있는경우
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
        else // 환경설정 파일이 없는경우
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
