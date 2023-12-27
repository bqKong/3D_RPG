using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playStats;

    //cinemachine
    private CinemachineFreeLook followCamera;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }


    public void RigisterPlayer(CharacterStats player)
    {
        playStats = player;

        //查找场景是否有这个camera
        followCamera = FindObjectOfType<CinemachineFreeLook>();

        //切换场景重新绑定虚拟相机的跟随目标
        if (followCamera != null)
        { 
            followCamera.Follow = playStats.transform.GetChild(2);
            followCamera.LookAt = playStats.transform.GetChild(2);
        }

    }

    public void AddObserver(IEndGameObserver observer) 
    {
        //不需要检测是否重复，因为只有每个敌人启用的时候才会注册，所以他们一定不会重复
        endGameObservers.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    { 
        endGameObservers.Remove(observer);
    }

    /// <summary>
    /// 广播方法
    /// </summary>
    public void NotifyObservers()
    { 
        //告诉每个观察者，要执行这个方法
        foreach (var observer in endGameObservers) 
        {
            observer.EndNotify();
        }
    }

    /// <summary>
    /// 获取第一个场景入口坐标
    /// </summary>
    /// <returns></returns>
    public Transform GetEntrance()
    {
        foreach (var item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == TransitionDestination.DestinationTag.ENTER)
            {
                return item.transform;
            }

        }

        return null;

    }

}
