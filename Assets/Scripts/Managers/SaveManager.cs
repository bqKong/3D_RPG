using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : Singleton<SaveManager>
{
    //存储当前人物所在场景
    private string sceneName = null;

    public string SceneName
    {
        get { return PlayerPrefs.GetString(sceneName); }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneController.Instance.TransitionToMain();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SavePlayerData();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadPlayerData();
        }

    }


    public void SavePlayerData()
    {
        Save(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    }

    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    }


    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);
        PlayerPrefs.SetString(key, jsonData);

        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);

        PlayerPrefs.Save();

    }

    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }

    }


}
