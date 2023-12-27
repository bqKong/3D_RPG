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

    /// <summary>
    /// 保存玩家数据
    /// </summary>
    public void SavePlayerData()
    {
        Save(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    }

    /// <summary>
    /// 加载玩家数据
    /// </summary>
    public void LoadPlayerData()
    {
        Load(GameManager.Instance.playStats.characterData, GameManager.Instance.playStats.characterData.name);
    }


    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data, true);

        //要保存 -- 玩家scriptable里面的数据
        PlayerPrefs.SetString(key, jsonData);

        //要保存的场景
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);

        //保存
        PlayerPrefs.Save();

    }

    /// <summary>
    /// 加载数据
    /// </summary>
    /// <param name="data"></param>
    /// <param name="key"></param>
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }

}
