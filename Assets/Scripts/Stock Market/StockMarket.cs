using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class StockMarket : MonoBehaviour
{
    #region Serialized Fields
    [Header("Candle Settings")]
    [Tooltip("Префаб свічки для створення")]
    [SerializeField] private GameObject candlePrefab;

    [Tooltip("Батьківський RectTransform для свічок")]
    [SerializeField] private RectTransform candleParent;

    [Header("UI Elements")]
    [Tooltip("Кнопка для ставки на зростання")]
    [SerializeField] private Button riseButton;

    [Tooltip("Кнопка для ставки на падіння")]
    [SerializeField] private Button fallButton;

    [Tooltip("Текст для відображення статусу ставки")]
    [SerializeField] private TMPro.TMP_Text statusText;
    #endregion

    #region Private Fields
    private CandleGenerator candleGenerator;
    private BetManager betManager;
    private bool isGenerating;
    private Coroutine currentBetCoroutine;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        candleGenerator = gameObject.AddComponent<CandleGenerator>();
        betManager = new BetManager();
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager не знайдений. Переконайтеся, що GameManager ініціалізований у попередній сцені.");
            return;
        }

        PlayerData playerData = GameManager.Instance.GetPlayerData();
        float initialPrice = playerData.StockPrice != 0 ? playerData.StockPrice : GameConfig.Instance.InitialStockPrice;

        candleGenerator.Initialize(candlePrefab, candleParent, new List<float> { initialPrice }, playerData);
        betManager.Initialize(playerData, candleGenerator, OnBetCompleted);

        riseButton.onClick.AddListener(() =>
        {
            MakeBet(true);
            AudioManager.Instance?.PlayButtonClickSound();
        });

        fallButton.onClick.AddListener(() =>
        {
            MakeBet(false);
            AudioManager.Instance?.PlayButtonClickSound();
        });

        GameManager.Instance.OnPlayerDataUpdated += UpdateButtons;
        StartCoroutine(candleGenerator.GenerateCandlesGradually(state => isGenerating = state));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerDataUpdated -= UpdateButtons;
        }
    }
    #endregion

    #region Betting Logic
    private void MakeBet(bool betOnRise)
    {
        if (isGenerating || currentBetCoroutine != null)
        {
            Debug.LogWarning("Ставка вже обробляється. Зачекайте завершення.");
            return;
        }

        if (statusText != null) statusText.text = "Обробка ставки...";
        currentBetCoroutine = StartCoroutine(betManager.ProcessBet(betOnRise));
    }

    private void OnBetCompleted()
    {
        StartCoroutine(ResetBetStateWithDelay());
    }

    private IEnumerator ResetBetStateWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        isGenerating = false;
        currentBetCoroutine = null;
        if (statusText != null) statusText.text = "Готово до нової ставки";
        UpdateButtons(GameManager.Instance.GetPlayerData());
    }
    #endregion

    #region UI Updates
    private void UpdateButtons(PlayerData playerData)
    {
        riseButton.interactable = !isGenerating;
        fallButton.interactable = !isGenerating;
    }
    #endregion
}