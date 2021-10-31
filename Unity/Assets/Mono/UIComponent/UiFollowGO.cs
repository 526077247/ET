using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollowGO : MonoBehaviour
{
    GameObject go;
    RectTransform rectTransform;
    GameObject canvas;
    public GameObject Go
    {
        get
        {
            return go;
        }
        set
        {
            go = value;
        }
    }

    private float height = 1.8f;//人物高度 用于定位头顶
    private float offSet = 0;//UI随着游戏物体与镜头的距离产生的偏移
    private double newBase = 1.1;//计算UI偏移offset的对数函数的底值

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = transform.Find("Canvas").gameObject;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Go != null)
        {
            Vector3 headPos = new Vector3(Go.transform.position.x, go.transform.position.y + height, go.transform.position.z);//人物头顶坐标
            Vector2 screenCoo;
            if (IsInView(headPos, out screenCoo))
            {
                canvas.SetActive(true);
                Vector3 screenPos = new Vector3(screenCoo.x * Screen.width, screenCoo.y * Screen.height);
                offSet = CalculateOffset(Vector3.Distance(Go.transform.position, Camera.main.transform.position));
                rectTransform.anchoredPosition = new Vector3(screenPos.x, screenPos.y + offSet, 0);
            }
            else
            {
                canvas.SetActive(false);
            }
        }
    }

    bool IsInView(Vector3 worldPos , out Vector2 viewPos)
    {
        Transform camTransform = Camera.main.transform;
        Vector3 dir = (worldPos - camTransform.position).normalized;
        float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面  
        viewPos = Camera.main.WorldToViewportPoint(worldPos);
        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// 根据距离计算UI偏移，越远越往上偏移。使用对数函数保证最终趋于一个稳定的值
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    float CalculateOffset(float distance)
    {
        //偏移最低为0，所以distance最少为1
        distance = Math.Max(1, distance);
        return (float)Math.Log(distance,newBase);
    }
}
