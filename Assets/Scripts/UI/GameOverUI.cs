using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : UIBase<GameOverUI>
{
    [SerializeField] private Button backToMenuButton;

    protected override void Awake()
    {
        base.Awake();
        backToMenuButton.onClick.AddListener(OnBackToMenuClick);
    }
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
        // 订阅 PlayerController 的死亡事件
        PlayerController.Instance.OnDeath += OnPlayerDeath;
    }
    protected override void OnDestroy()
    {
        // 取消订阅
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnDeath -= OnPlayerDeath;

        backToMenuButton.onClick.RemoveListener(OnBackToMenuClick);
    }    
    private void OnPlayerDeath()
    {
        Enter();  // 打开面板
        // 禁用玩家输入，解锁鼠标
        PlayerController.Instance.DisableInput();
    }
    // 实现抽象方法
    protected override void OnEableButtons()
    {
        backToMenuButton.interactable = true;
    }

    protected override void DisableButtons()
    {
        backToMenuButton.interactable = false;
    }

    private void OnBackToMenuClick()
    {
        SceneManager.LoadScene("GameStart");
    }
}