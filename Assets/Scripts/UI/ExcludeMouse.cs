using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//躲避鼠标
public class ExcludeMouse : MonoBehaviour
{
    public float avoideRadius = 150f;//影响半径
    public float avoidForce = 500f;//躲避速度
    public float returnForce = 2f;//返回原位速度

    private Vector2 originaPos; //引力中心
    private RectTransform rectTransform;
    private Camera uiCamera;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        uiCamera = GetComponentInParent<Canvas>().worldCamera;
        originaPos = rectTransform.anchoredPosition;
    }
    private void Update()
    {
        //鼠标相对于按钮的本地坐标
        Vector2 mousePosInRect;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            uiCamera,
            out mousePosInRect );

        //指针离按钮距离
        float distance = mousePosInRect.magnitude;

        Vector2 movement = Vector2.zero;
        if(distance < avoideRadius)
        {
            //计算躲避方向和力度
            Vector2 avoidDir = -mousePosInRect.normalized;
            //鼠标离按钮越近，百分比越大，躲避力度越大
            float avoidPercent = (avoideRadius -  distance) / avoideRadius;
            movement += avoidDir * avoidForce * avoidPercent * Time.deltaTime;
        }

        movement += (originaPos - rectTransform.anchoredPosition) * returnForce * Time.deltaTime;

        //应用移动
        rectTransform.anchoredPosition += movement;
    }

}
