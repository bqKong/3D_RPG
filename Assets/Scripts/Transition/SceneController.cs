using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneController : Singleton<SceneController>,IEndGameObserver
{

    public GameObject playerPrefab;

    public SceneFader sceneFaderPrefab;

    private GameObject player;
    private NavMeshAgent playerAgent;

    bool fadeFinished;


    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        GameManager.Instance.AddObserver(this);
        fadeFinished = true;
    }

    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionPoint.TransitionType.SameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

            case TransitionPoint.TransitionType.DifferentScene:
                //异步加载
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;

        }
    }

    /// <summary>
    /// 同场景或跨场景传送
    /// </summary>
    /// <param name="sceneName">场景名字</param>
    /// <param name="destinationTag">要去的场景的标签</param>
    /// <returns></returns>
    private IEnumerator Transition(string sceneName, TransitionDestination.DestinationTag destinationTag)
    {

        //TODO:保存数据
        SaveManager.Instance.SavePlayerData();

        //不同场景
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            //FIXME:可以加入fader
            //yieled return 是否在这一帧等待时间完成
            yield return SceneManager.LoadSceneAsync(sceneName);

            yield return Instantiate(playerPrefab,GetDestination(destinationTag).transform.position,
                GetDestination(destinationTag).transform.rotation);
            //读取数据
            SaveManager.Instance.LoadPlayerData();
            yield break;

        }
        else
        {
            //同场景就获取player，修改player的坐标
            player = GameManager.Instance.playStats.gameObject;
            playerAgent = player.GetComponent<NavMeshAgent>();

            playerAgent.enabled = false;
            //设置位置和旋转角度
            //获取挂载TransitionDestination那个组件的坐标和旋转方向
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position,
              GetDestination(destinationTag).transform.rotation);

            playerAgent.enabled = true;
            yield return null;
        }

    }

    /// <summary>
    /// 获取该终点标签的位置坐标
    /// </summary>
    /// <param name="destinationTag"></param>
    /// <returns></returns>
    private TransitionDestination GetDestination(TransitionDestination.DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();

        for (int i = 0; i < entrances.Length; i++)
        {
            if (entrances[i].destinationTag == destinationTag)
            {
                return entrances[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Continue Game
    /// </summary>
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(SaveManager.Instance.SceneName));
    }

    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }

    /// <summary>
    /// New Game
    /// </summary>
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("SimpleNaturePack_Demo"));
    }

    IEnumerator LoadLevel(string scene)
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);

        if (scene != "")
        {
            //淡出
            yield return StartCoroutine(fade.FadeOut(2.5f));

            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab,GameManager.Instance.GetEntrance().position,
                GameManager.Instance.GetEntrance().rotation);

            //保存游戏数据
            SaveManager.Instance.SavePlayerData();

            //淡入
            yield return StartCoroutine(fade.FadeIn(2.5f));

            yield break;
        }
        

    }

    /// <summary>
    /// 加载主菜单
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadMain()
    {
        SceneFader fade = Instantiate(sceneFaderPrefab);
        yield return StartCoroutine(fade.FadeOut(2f));

        yield return SceneManager.LoadSceneAsync("Main");

        yield return StartCoroutine(fade.FadeIn(2f));
        yield break;
    }

    /// <summary>
    /// 观察者方法
    /// </summary>
    public void EndNotify()
    {
        //TransitionToMain();
        if (fadeFinished)
        { 
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
        
    }
}
