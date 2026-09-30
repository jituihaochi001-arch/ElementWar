using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingCanvas : UIBase<LoadingCanvas>
{
    public Image fillImage;
    public TMP_Text percent;

    protected override void Awake()
    {
        base.Awake();
        gameObject.SetActive(false);
    }
    protected override void DisableButtons()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnEableButtons()
    {
        throw new System.NotImplementedException();
    }

    public void SceneToLoad(string sceneName)
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadProgress(sceneName));
    }

    //异步加载协程
    IEnumerator LoadProgress(string sceneName)
    {
        gameObject.SetActive(true) ;
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = true;

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress/0.9f);

            fillImage.fillAmount = progress;
            percent.text = (progress * 100).ToString() + "%";
            yield return null;
        }
        fillImage.fillAmount = 1f;
        percent.text = "100%";
        gameObject.SetActive(false);
    }
}
