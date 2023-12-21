using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance => instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = (T)this;
        }
    }

    public static bool IsInitialized { get { return instance != null; } }

    //如果一个场景有多个单例是要将它销毁的
    /// <summary>
    /// 销毁单例
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (instance == this)
        { 
            instance = null;
        }
    }

}
