using UnityEngine;

public class GameConfig : MonoBehaviour
{
    public static GameConfig Instance { get; private set; }

    #region Game Mechanics Settings
    [Header("Game Mechanics Settings")]
    [Tooltip("Початкове значення за клік")]
    public double InitialClickValue = 1.0;

    [Tooltip("Початкова вартість покращення подвійного кліку")]
    public double InitialDoubleClickCost = 50.0;

    [Tooltip("Множник вартості покращення подвійного кліку")]
    public double DoubleClickCostMultiplier = 2.5;

    [Space]
    [Tooltip("Початкова вартість покращення пасивного доходу")]
    public double InitialPassiveIncomeCost = 150.0;

    [Tooltip("Множник вартості покращення пасивного доходу")]
    public double PassiveIncomeCostMultiplier = 2.0;

    [Tooltip("Збільшення пасивного доходу за рівень")]
    public double PassiveIncomeIncrement = 0.5;

    [Tooltip("Фактор динамічного масштабування вартості")]
    public double DynamicCostScalingFactor = 0.0001;
    #endregion

    #region Stock Market Settings
    [Header("Stock Market Settings")]
    [Tooltip("Початкова ціна акції")]
    public float InitialStockPrice = 100f;

    [Tooltip("Максимальна варіація ціни (для старої моделі)")]
    public float StockPriceVariation = 5f;

    [Space]
    [Tooltip("Ширина свічки в пікселях")]
    public float CandleWidth = 15f;

    [Tooltip("Затримка між появою свічок (у секундах)")]
    public float CandleDelay = 0.2f;

    [Tooltip("Відступ для свічок (у відсотках від висоти)")]
    public float CandlePadding = 0.1f;

    [Space]
    [Tooltip("Виграш за вдалу ставку")]
    public float BetWinAmount = 100f;

    [Tooltip("Програш за невдалу ставку")]
    public float BetLossAmount = 100f;

    [Tooltip("Кількість свічок за цикл")]
    public int CandlesPerCycle = 10;

    [Tooltip("Тривалість анімації свічки (у секундах)")]
    public float CandleAnimationDuration = 0.5f;
    #endregion

    #region Stock Market Simulation Settings
    [Header("Stock Market Simulation Settings")]
    [Tooltip("Базова волатильність (у відсотках)")]
    public float BaseVolatility = 0.02f;

    [Tooltip("Ймовірність зміни тренду (0-1)")]
    public float TrendChangeProbability = 0.1f;

    [Tooltip("Тренд зростання (у відсотках за цикл)")]
    public float BullishDrift = 0.01f;

    [Tooltip("Тренд падіння (у відсотках за цикл)")]
    public float BearishDrift = -0.01f;

    [Tooltip("Максимальна кількість свічок в історії")]
    public int MaxPriceHistory = 50;
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}