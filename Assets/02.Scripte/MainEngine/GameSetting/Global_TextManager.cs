using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Global_GameSetting;

public class Global_TextManager : MonoBehaviour
{
    protected static TextAsset jsonData;
    protected static JArray json;

    private static List<string> TextList = new List<string>();
    public delegate void EventTextChange();
    /// <summary>
    /// (델리게이트) 텍스트를 즉시 바꿔주는 기능입니다. 델리게이트 호출을 잘활용해야합니다. 
    /// </summary>
    public static EventTextChange dEventTextChange;

    private void Awake()
    {
        jsonData = Resources.Load<TextAsset>("SettingData/Text/TextDataTable");
        json = JArray.Parse(jsonData.text);
    }

    private const int TextCount = 51;

    public static void SetLangauge(eGS_Language language)
    {
        string temp_langCode = "KR";
        switch (language)
        {
            case eGS_Language.Korean:
                temp_langCode = "KR";
                break;
            case eGS_Language.English:
                temp_langCode = "EN";
                break;
            case eGS_Language.Japanese:
                temp_langCode = "JP";
                break;
        }

        for (int i = 0; i < TextCount; i++)
        {
            TextList.Add(json[i][temp_langCode].ToObject<string>());
        }
        dEventTextChange();
    }

    private static string GetText(int index)
    {
        return TextList[index];
    }

}
