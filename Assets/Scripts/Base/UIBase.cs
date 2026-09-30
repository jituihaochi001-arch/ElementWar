using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIBase<T> : SingleMonoBase<T> where T : UIBase<T>
{
    private Animator anim;
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }


    protected virtual void Start() { }

  
    //显示UI
    public virtual void Enter()
    {
        gameObject.SetActive(true);
        anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        StartCoroutine(_Enter());
    }
    private IEnumerator _Enter()
    {       
        anim.Play("FadeIn");      
        yield return new WaitUntil(() => IsAnimationBreak());
        anim.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        OnEableButtons();
    }

    //隐藏UI
    public virtual void Exit(Action action)
    {
        DisableButtons();
        anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
        StartCoroutine(_Exit(action));
    }
    private  IEnumerator _Exit(Action action)
    {      
        anim.Play("FadeOut");
        action?.Invoke();
        yield return null;
        yield return new WaitUntil(() => IsAnimationBreak());  // 等动画播完  
       
        gameObject.SetActive(false);
    }
    //播放动画
    public void PlayeAnimation(string animationName)
    {
        anim.CrossFadeInFixedTime(animationName,0);
    }

    //判断当前动画是否播放完毕
    protected bool IsAnimationBreak()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        return info.normalizedTime >= 1.0f && !anim.IsInTransition(0);
    }
    protected abstract void DisableButtons();
    protected abstract void OnEableButtons();

}
