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
    /// (���)
    /// <br>������ ó�� �ε�Ǹ鼭 �ʿ��� ������ �������ݴϴ�. </br>
    /// <br>������ ������ ���ξ����� ��ȯ�˴ϴ�. </br>
    /// </summary>
    private void GameInit()
    {
        _logManager.dataPath = Application.persistentDataPath + "/ErrorLog.txt"; // �α׸Ŵ����� �ؽ�Ʈ���� �����θ� �����մϴ�. 

        _teamSetting.Init(); // �������� ����εǾ��ִ��� Ȯ���մϴ�. 

        FinishSetting = true; // ��� ������ ������ ���ξ��� �ҷ��ɴϴ�.
    }

    /// <summary>
    /// (���) 
    /// <br>���ϴ� ������ ��ȯ��Ű�� �Լ��Դϴ�. </br>
    /// </summary>
    /// <param name="sceneName">��ȯ�ϰ��� �ϴ� ���� �������ݴϴ�. </param>
    /// <param name="addScene">���� ���� �ٸ����� �߰������� �����մϴ�. </param>
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
