using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class EventVector3 : UnityEvent<Vector3> { }

public class MouseManager : MonoBehaviour
{
    public EventVector3 OnMouseClicked;

    RaycastHit hitInfo;

    //和上面的写法效果一样
    //public UnityEvent<Vector3> OnMouse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetCursorTexture();
        MouseControl();

    }

    void SetCursorTexture()
    { 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var state = Physics.Raycast(ray, out hitInfo);
        if(state)
        {
           
        }

    }

    void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        { 
            if(hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                OnMouseClicked?.Invoke(hitInfo.point);
            }

        }


    }

}
