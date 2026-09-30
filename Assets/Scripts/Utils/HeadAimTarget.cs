using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadAimTarget : MonoBehaviour
{
    private Camera mainCamera;
    private Plane plane;

    private Vector3 targetPos = Vector3.zero;
    [Tooltip("移动到鼠标的速度")]
    public float speed = 5f;

    private void Awake()
    {
        mainCamera = Camera.main;
        //创建一个垂直于相机射线的平面
        plane = new Plane(mainCamera.transform.forward,transform.position);
    }
    private void Update()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if(plane.Raycast(cameraRay,out enter))
        {
            Vector3 hitPoint = cameraRay.GetPoint(enter);
            targetPos = hitPoint;
        }
        float x = Mathf.Lerp(transform.position.x , targetPos.x , speed * Time.deltaTime);
        float y = Mathf.Lerp(transform.position.y , targetPos.y , speed * Time.deltaTime);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
