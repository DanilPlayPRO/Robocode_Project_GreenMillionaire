using UnityEngine;

public class UpgradeSystem
{
    #region Fields
    private PlayerData playerData;
    private System.Action onUpgradeApplied;
    public float GetClickMultiplier => playerData.DoubleClickLevel > 0 ? 2f : 1f;
    #endregion

    #region Initialization
    public void Initialize(PlayerData data, System.Action onUpgradeAppliedCallback)
    {
        playerData = data;
        onUpgradeApplied = onUpgradeAppliedCallback;
    }
    #endregion

    #region Upgrade Logic
    public void UpgradeDoubleClick()
    {
        if (playerData.Balance < playerData.DoubleClickUpgradeCost)
        {
            Debug.LogWarning("Недостатньо коштів для покращення подвійного кліку.");
            return;
        }

        playerData.SpendBalance(playerData.DoubleClickUpgradeCost);
        playerData.DoubleClickLevel++;
        playerData.DoubleClickUpgradeCost *= GameConfig.Instance.DoubleClickCostMultiplier;
        playerData.DoubleClickUpgradeCost += playerData.DoubleClickLevel * GameConfig.Instance.DynamicCostScalingFactor;

        onUpgradeApplied?.Invoke();
    }

    public void UpgradePassiveIncome()
    {
        if (playerData.Balance < playerData.PassiveIncomeUpgradeCost)
        {
            Debug.LogWarning("Недостатньо коштів для покращення пасивного доходу.");
            return;
        }

        playerData.SpendBalance(playerData.PassiveIncomeUpgradeCost);
        playerData.IncreasePassiveIncome(GameConfig.Instance.PassiveIncomeIncrement);
        playerData.PassiveIncomeUpgradeCost *= GameConfig.Instance.PassiveIncomeCostMultiplier;

        onUpgradeApplied?.Invoke();
    }
    #endregion
}