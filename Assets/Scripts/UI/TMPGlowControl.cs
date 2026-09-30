using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

//控制文字发光
public class TMPGlowControl : MonoBehaviour,IPointerEnterHandler
{
    public TMP_Text textComponent;
    [Tooltip("最大发光值")]
    public float maxGlowPower = 1;
    [Tooltip("总发光时间")]
    public float glowTime = 0.2f;
    [Tooltip("淡入发光时间")]
    public float fadeInTime = 0.05f;
    private Material glowMaterial;//文本材质

    private void Awake()
    {
        glowMaterial = textComponent.fontMaterial;

    }
    private void Start()
    {
        DisableUnderLay();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        //让文字产生发光效果
        StopAllCoroutines();
        StartCoroutine(AnimayeGlowEffect());
        //让文字产生阴影效果
        EnableUnderLay();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        
        DisableUnderLay();
        
    }
    //进入发光效果
    private IEnumerator AnimayeGlowEffect()
    {
        //淡入发光效果
        float timer = 0;
        while(timer < fadeInTime)
        {
            float t = timer / fadeInTime;
            float currentPower = Mathf.Lerp(0f,maxGlowPower,t);
            glowMaterial.SetFloat("_GlowPower",currentPower);
            timer += Time.deltaTime;
            yield return null;
        }
        //强制设置为最大发光强度
        glowMaterial.SetFloat("_GlowPower", maxGlowPower);

        //淡出发光效果
        timer = 0;
        float fadeOutTime = glowTime - fadeInTime;//计算淡出时间
        while(timer < fadeOutTime)
        {
            float t = timer / fadeOutTime;
            float currentPower = Mathf.Lerp(maxGlowPower,0,t);
            glowMaterial.SetFloat("_GlowPower", currentPower);
            timer += Time.deltaTime;
            yield return null;
        }
        glowMaterial.SetFloat("_GlowPower", 0f);
    }

    private void EnableUnderLay()
    {
        if(glowMaterial != null)
        {
            glowMaterial.EnableKeyword("UNDERLAY_ON");
        }
    }
    public void DisableUnderLay() 
    {
        if (glowMaterial != null)
        {
            glowMaterial.DisableKeyword("UNDERLAY_ON");
        }
    }
}
