using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Ã· æ≤Àµ•
public class TipMenuUI : UIBase<TipMenuUI>
{
    public Button BtnYes;

    protected override void Awake()
    {
        base.Awake();
        BtnYes.onClick.AddListener(() => {
            Exit(() => { MainMenuUI.Instance.Enter(); });
            });
    }
    protected override void Start()
    {
        gameObject.SetActive(false);
    }
    protected override void OnEableButtons()
    {
        BtnYes.interactable = true;
    }
    protected override void DisableButtons()
    {
        BtnYes.interactable = false;
    }

}
