using UnityEngine;
using TMPro;

public class StockMarketUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text balanceText;

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager не знайдений. Переконайтеся, що GameManager ініціалізований у попередній сцені.");
            return;
        }

        GameManager.Instance.OnPlayerDataUpdated += UpdateUI;
        UpdateUI(GameManager.Instance.GetPlayerData());
    }

    private void UpdateUI(PlayerData playerData)
    {
        if (balanceText != null)
        {
            balanceText.text = $"Баланс: {playerData.Balance:F2} $";
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDataUpdated -= UpdateUI;
        }
    }
}