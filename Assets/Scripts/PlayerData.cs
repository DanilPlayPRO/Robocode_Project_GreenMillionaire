using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    #region Player Stats
    public double Balance;
    public double ClickValue;
    public double PassiveIncome;
    public double DoubleClickUpgradeCost;
    public int DoubleClickLevel;
    public double PassiveIncomeUpgradeCost;
    #endregion

    #region Stock Market Data
    public float StockPrice;
    public List<float> StockPriceHistory;
    public bool IsBullishTrend;
    #endregion

    public PlayerData()
    {
        // Ініціалізуємо лише ті поля, які не залежать від GameConfig
        Balance = 0;
        DoubleClickLevel = 0;
        StockPrice = 0;
        StockPriceHistory = new List<float>();
        IsBullishTrend = true;
    }

    public void Initialize()
    {
        if (GameConfig.Instance == null)
        {
            Debug.LogError("GameConfig не ініціалізований. Переконайтеся, що GameConfig присутній у сцені.");
            return;
        }

        ClickValue = GameConfig.Instance.InitialClickValue;
        PassiveIncome = 0;
        DoubleClickUpgradeCost = GameConfig.Instance.InitialDoubleClickCost;
        PassiveIncomeUpgradeCost = GameConfig.Instance.InitialPassiveIncomeCost;
    }

    public void AddBalance(double amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Спроба додати від'ємне значення до балансу.");
            return;
        }
        Balance += amount;
    }

    public void SpendBalance(double amount)
    {
        if (amount > Balance)
        {
            Debug.LogWarning("Недостатньо коштів для покупки.");
            return;
        }
        Balance -= amount;
    }

    public void IncreasePassiveIncome(double amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Спроба додати від'ємне значення до пасивного доходу.");
            return;
        }
        PassiveIncome += amount;
    }
}