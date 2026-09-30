using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [Tooltip("血条填充块")]
    public Image healthSlider;

    private Transform cameraTransform;
    private void Start()
    {
        cameraTransform = Camera.main.transform;   
        healthSlider.fillAmount = 1;
    }
    private void Update()
    {
        //计算UI到摄像机的方向
        Vector3 dir = cameraTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-dir);
    }

    public void UpdateHealthBar(float healthRatio)
    {
        healthSlider.fillAmount = healthRatio;
    }
}
