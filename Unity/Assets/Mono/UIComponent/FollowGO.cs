using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowGO : MonoBehaviour
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

    private float height = 1.8f;//����߶� ���ڶ�λͷ��
    private float offSet = 0;//UI������Ϸ�����뾵ͷ�ľ��������ƫ��
    private double newBase = 1.1;//����UIƫ��offset�Ķ��������ĵ�ֵ

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
            Vector3 headPos = new Vector3(Go.transform.position.x, go.transform.position.y + height, go.transform.position.z);//����ͷ������
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
        float dot = Vector3.Dot(camTransform.forward, dir);     //�ж������Ƿ������ǰ��  
        viewPos = Camera.main.WorldToViewportPoint(worldPos);
        if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// ���ݾ������UIƫ�ƣ�ԽԶԽ����ƫ�ơ�ʹ�ö���������֤��������һ���ȶ���ֵ
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    float CalculateOffset(float distance)
    {
        //ƫ�����Ϊ0������distance����Ϊ1
        distance = Math.Max(1, distance);
        return (float)Math.Log(distance,newBase);
    }
}
