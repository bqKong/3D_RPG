using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playStats;

    List<IEndGameObserver> endGameObservers = new List<IEndGameObserver>();

    public void RigisterPlayer(CharacterStats player)
    {
        playStats = player;
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

}
