using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI :UIBase<MainMenuUI>
{
    public Button onlineBtn;
    public Button continueBtn;
    public Button newGameBtn;
    public Button loadBtn;   
    public Button characterBtn;
    public Button clothesBtn;
    public Button settingBtn;
    public Button exitBtn;

    protected override void Awake()
    {
        base.Awake();
        onlineBtn.onClick.AddListener(() => {ShowTipPanel(); });
        continueBtn.onClick.AddListener(() => {  ShowTipPanel(); });
        loadBtn.onClick.AddListener(() => {  ShowTipPanel(); });
        newGameBtn.onClick.AddListener(() => { LoadingCanvas.Instance.SceneToLoad("Game"); });//载入新游戏
        characterBtn.onClick.AddListener(() => {  ShowTipPanel(); });
        clothesBtn.onClick.AddListener(() => {  ShowTipPanel(); });
        settingBtn.onClick.AddListener(() => {  ShowTipPanel(); });
        exitBtn.onClick.AddListener(() => {
            Exit(() => { ExitMenuUI.Instance.Enter(); }); });//进入退出面板
    }

    protected override void Start()
    {
        base.Start();
        Enter();
    }
    private void ShowTipPanel()
    {
        Exit(() => { TipMenuUI.Instance.Enter(); });
    }
    protected override void OnEableButtons()
    {
        onlineBtn.interactable = true;
    }
    protected override void DisableButtons()
    {
        onlineBtn.interactable = false;
    }
   
}
