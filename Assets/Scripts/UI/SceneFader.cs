using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    CanvasGroup canvasGroup;

    public float fadeInDuration;

    public float fadeOutDuration;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        DontDestroyOnLoad(gameObject);
    }

    //协程套协程
    public IEnumerator FadeOutIn( )
    {
        yield return FadeOut(fadeOutDuration);
        yield return FadeIn(fadeInDuration);
    }


    public IEnumerator FadeOut(float time)
    {
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / time;
            yield return null;
        }

    }

    public IEnumerator FadeIn(float time)
    {
        while (canvasGroup.alpha != 0)
        {
            canvasGroup.alpha -= Time.deltaTime / time;
            yield return null;
        }

        //切换完后销毁，不会产生多个克隆物体
        Destroy(gameObject);
    }


}
