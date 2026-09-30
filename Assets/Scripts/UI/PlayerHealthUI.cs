using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text healthText;

    private PlayerModel currentModel;

    private void Start()
    {
        // 订阅"切换角色"事件
        PlayerController.Instance.OnPlayerModelChanged += OnPlayerModelChanged;

        // 初始化当前角色
        OnPlayerModelChanged(PlayerController.Instance.currentModel);
    }

    private void OnDestroy()
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnPlayerModelChanged -= OnPlayerModelChanged;

        UnsubscribeCurrentModel();
    }

    /// <summary>
    /// 切换角色时调用
    /// </summary>
    private void OnPlayerModelChanged(PlayerModel newModel)
    {
        // 1. 取消旧角色的监听
        UnsubscribeCurrentModel();

        // 2. 换到新角色
        currentModel = newModel;

        // 3. 订阅新角色的血量变化
        if (currentModel != null)
        {
            currentModel.OnHealthChanged += UpdateHealthUI;
            // 立刻刷新一次，显示新角色的血量
            UpdateHealthUI(currentModel.currentHealth, currentModel.maxHealth);
        }
    }

    private void UnsubscribeCurrentModel()
    {
        if (currentModel != null)
            currentModel.OnHealthChanged -= UpdateHealthUI;
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (fillImage != null)
            fillImage.fillAmount = (float)current / max; 

        if (healthText != null)
            healthText.text = $"HP:{current}/{max}";
    }
}