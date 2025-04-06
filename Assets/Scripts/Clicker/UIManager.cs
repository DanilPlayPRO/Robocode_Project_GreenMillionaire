using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    #region Serialized Fields
    [Header("UI Elements")]
    [Tooltip("Текст для відображення балансу гравця")]
    [SerializeField] private TMP_Text balanceText;

    [Tooltip("Кнопка для кліку")]
    [SerializeField] private Button clickButton;

    [Header("Double Click Upgrade")]
    [Tooltip("Кнопка для покращення подвійного кліку")]
    [SerializeField] private Button doubleClickButton;

    [Tooltip("Текст для відображення вартості покращення подвійного кліку")]
    [SerializeField] private TMP_Text doubleClickCostText;

    [Header("Passive Income Upgrade")]
    [Tooltip("Кнопка для покращення пасивного доходу")]
    [SerializeField] private Button passiveIncomeButton;

    [Tooltip("Текст для відображення вартості покращення пасивного доходу")]
    [SerializeField] private TMP_Text passiveIncomeCostText;

    [Tooltip("Текст для відображення поточного пасивного доходу")]
    [SerializeField] private TMP_Text passiveIncomeText;
    #endregion

    #region Private Fields
    private GameManager gameManager;
    private UpgradeSystem upgradeSystem;
    #endregion

    #region Initialization
    public void Initialize(GameManager manager, UpgradeSystem system)
    {
        gameManager = manager;
        upgradeSystem = system;

        if (!ValidateUIElements())
        {
            Debug.LogError("Один або кілька UI елементів не призначені в інспекторі. UIManager не може ініціалізуватися.");
            return;
        }

        // Додаємо звуки для кнопок
        clickButton.onClick.AddListener(() =>
        {
            gameManager.HandleClick();
            AudioManager.Instance?.PlayButtonClickSound();
        });

        doubleClickButton.onClick.AddListener(() =>
        {
            upgradeSystem.UpgradeDoubleClick();
            AudioManager.Instance?.PlayButtonClickSound();
        });

        passiveIncomeButton.onClick.AddListener(() =>
        {
            upgradeSystem.UpgradePassiveIncome();
            AudioManager.Instance?.PlayButtonClickSound();
        });

        gameManager.OnPlayerDataUpdated += UpdateUI;
        UpdateUI(gameManager.GetPlayerData());
    }
    #endregion

    #region UI Updates
    private void UpdateUI(PlayerData playerData)
    {
        if (balanceText != null)
            balanceText.text = $"Баланс: {playerData.Balance:F2} $";
        if (doubleClickCostText != null)
            doubleClickCostText.text = $"Вартість: {playerData.DoubleClickUpgradeCost:F2} $";
        if (passiveIncomeCostText != null)
            passiveIncomeCostText.text = $"Вартість: {playerData.PassiveIncomeUpgradeCost:F2} $";
        if (passiveIncomeText != null)
            passiveIncomeText.text = $"Пасивний дохід: {playerData.PassiveIncome:F2} $/с";

        if (doubleClickButton != null)
            doubleClickButton.interactable = playerData.Balance >= playerData.DoubleClickUpgradeCost;
        if (passiveIncomeButton != null)
            passiveIncomeButton.interactable = playerData.Balance >= playerData.PassiveIncomeUpgradeCost;
    }
    #endregion

    #region Validation
    private bool ValidateUIElements()
    {
        bool isValid = true;

        if (balanceText == null)
        {
            Debug.LogError("balanceText не призначений в інспекторі.");
            isValid = false;
        }
        if (clickButton == null)
        {
            Debug.LogError("clickButton не призначений в інспекторі.");
            isValid = false;
        }
        if (doubleClickButton == null)
        {
            Debug.LogError("doubleClickButton не призначений в інспекторі.");
            isValid = false;
        }
        if (doubleClickCostText == null)
        {
            Debug.LogError("doubleClickCostText не призначений в інспекторі.");
            isValid = false;
        }
        if (passiveIncomeButton == null)
        {
            Debug.LogError("passiveIncomeButton не призначений в інспекторі.");
            isValid = false;
        }
        if (passiveIncomeCostText == null)
        {
            Debug.LogError("passiveIncomeCostText не призначений в інспекторі.");
            isValid = false;
        }
        if (passiveIncomeText == null)
        {
            Debug.LogError("passiveIncomeText не призначений в інспекторі.");
            isValid = false;
        }

        return isValid;
    }
    #endregion

    #region Unity Lifecycle Methods
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerDataUpdated -= UpdateUI;
        }
    }
    #endregion
}