using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//[Serializable]
//public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : Singleton<MouseManager>
{
    // public static MouseManager Instance;

    [Header("鼠标图标")]
    public Texture2D point, doorway, attack, target, arrow;

    //public EventVector3 OnMouseClicked;
    //和上面的写法效果一样
    //public UnityEvent<Vector3> OnMouseClicked;

    //自定义的鼠标点击事件
    //思路：要移动那得有个目标的position，要攻击得有目标的gameobject
    public UnityAction<Vector3> OnMouseClicked;
    public event UnityAction<GameObject> OnEnemyClicked;

    RaycastHit hitInfo;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
        MouseControl();
    }

    /// <summary>
    /// 设置鼠标的图案
    /// </summary>
    void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var state = Physics.Raycast(ray, out hitInfo);
        if (state)
        {
            //切换鼠标贴图
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16, 16), CursorMode.Auto);
                    break;

                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16, 16), CursorMode.Auto);
                    break;

                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16, 16), CursorMode.Auto);
                    break;

                default:
                    Cursor.SetCursor(arrow, new Vector2(16, 16), CursorMode.Auto);
                    break;
            }

        }

    }

    //思路：该事件在内部广播调用，订阅器应在PlayerController身上
    /// <summary>
    /// 事件调用，事件触发
    /// </summary>
    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

            if (hitInfo.collider.gameObject.tag.Equals("Enemy"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.tag.Equals("Attackable"))
            {
                OnEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }

            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

        }

    }

}
