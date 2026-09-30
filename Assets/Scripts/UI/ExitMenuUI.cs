using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class ExitMenuUI : UIBase<ExitMenuUI>
{
    public Button yesBtn;
    public Button noBtn;

    protected override void Awake()
    {
        base.Awake();
        yesBtn.onClick.AddListener(() =>
        { 
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;  // 在编辑器里停止播放
        #else
            Application.Quit();  // 在打包后的游戏里退出
        #endif
        });          
        noBtn.onClick.AddListener(() =>{Exit(() => { MainMenuUI.Instance.Enter(); });});
    }
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
    }

    protected override void OnEableButtons()
    {
        yesBtn.interactable = true;
        noBtn.interactable = true;
    }
    protected override void DisableButtons()
    {
        yesBtn.interactable = false;
        noBtn.interactable = false;
    }
  
}
