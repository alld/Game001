using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string gameVersion = "0.00";
    public static bool FinishSetting = false; 

    public enum SceneN
    {
        Intro,
        Main
    }
    private string[] sceneName =
    {
        "00.Intro",
        "01.Main",
    };
    private List<string> currentScene = new List<string>();

    public static GameManager _instance = null;
    public Global_TeamSetting _teamSetting = null;
    public Global_LogManager _logManager = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this.gameObject); }

        GameInit();
    }

    /// <summary>
    /// (기능)
    /// <br>게임이 처음 로드되면서 필요한 값들을 설정해줍니다. </br>
    /// <br>설정이 끝나면 메인씬으로 전환됩니다. </br>
    /// </summary>
    private void GameInit()
    {
        _logManager.dataPath = Application.persistentDataPath + "/ErrorLog.txt"; // 로그매니저의 텍스트파일 저장경로를 지정합니다. 

        _teamSetting.Init(); // 팀설정이 제대로되어있는지 확인합니다. 

        FinishSetting = true; // 모든 설정이 끝나면 메인씬을 불러옵니다.
    }

    /// <summary>
    /// (기능) 
    /// <br>원하는 씬으로 전환시키는 함수입니다. </br>
    /// </summary>
    /// <param name="sceneName">전환하고자 하는 씬을 지정해줍니다. </param>
    /// <param name="addScene">현재 씬에 다른씬을 추가할지를 결정합니다. </param>
    public void ChangeScene(SceneN sceneName, bool addScene = false)
    {
        string changeSceneName = this.sceneName[(int)sceneName];
        if (addScene == true)
        {
            SceneManager.LoadSceneAsync(changeSceneName, LoadSceneMode.Additive);
            currentScene.Add(changeSceneName);
        }
        else
        {
            SceneManager.LoadSceneAsync(changeSceneName, LoadSceneMode.Single);
            foreach (var scene in currentScene)
            {
                SceneManager.UnloadSceneAsync(scene);
            }
            currentScene.Clear();
            currentScene.Add(changeSceneName);
        }
    }
}
