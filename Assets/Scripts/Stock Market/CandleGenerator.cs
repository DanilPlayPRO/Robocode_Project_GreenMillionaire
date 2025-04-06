using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CandleGenerator : MonoBehaviour
{
    #region Fields
    [Header("Candle Settings")]
    [Tooltip("Префаб свічки для створення")]
    [SerializeField] private GameObject candlePrefab;

    [Tooltip("Батьківський RectTransform для свічок")]
    [SerializeField] private RectTransform candleParent;

    private List<float> prices;
    private float minPrice;
    private float maxPrice;
    private PlayerData playerData;
    private bool isBullishTrend;
    #endregion

    #region Initialization
    public void Initialize(GameObject prefab, RectTransform parent, List<float> initialPrices, PlayerData data)
    {
        candlePrefab = prefab;
        candleParent = parent;
        prices = initialPrices;
        playerData = data;
        isBullishTrend = playerData.IsBullishTrend;
    }
    #endregion

    #region Candle Generation
    public IEnumerator GenerateCandlesGradually(System.Action<bool> onGenerationStateChanged)
    {
        onGenerationStateChanged?.Invoke(true);

        foreach (Transform child in candleParent)
        {
            Destroy(child.gameObject);
        }

        List<float> newPrices = new List<float> { prices[prices.Count - 1] };
        float dt = 1f / GameConfig.Instance.CandlesPerCycle;
        float drift = isBullishTrend ? GameConfig.Instance.BullishDrift : GameConfig.Instance.BearishDrift;
        float volatility = GameConfig.Instance.BaseVolatility;

        for (int i = 0; i < GameConfig.Instance.CandlesPerCycle; i++)
        {
            float currentPrice = newPrices[newPrices.Count - 1];
            float randomFactor = RandomGaussian(0, 1);
            float priceChange = (drift - 0.5f * volatility * volatility) * dt + volatility * Mathf.Sqrt(dt) * randomFactor;
            float newPrice = currentPrice * Mathf.Exp(priceChange);
            newPrices.Add(newPrice);
        }

        minPrice = Mathf.Min(newPrices.ToArray()) - 2f;
        maxPrice = Mathf.Max(newPrices.ToArray()) + 2f;

        float parentWidth = candleParent.rect.width;
        float parentHeight = candleParent.rect.height;
        float candleSpacing = parentWidth / GameConfig.Instance.CandlesPerCycle;

        for (int i = 0; i < GameConfig.Instance.CandlesPerCycle; i++)
        {
            float open = i == 0 ? prices[prices.Count - 1] : newPrices[i];
            float close = newPrices[i + 1];

            float high = Mathf.Max(open, close);
            float low = Mathf.Min(open, close);
            for (int j = 0; j < 5; j++)
            {
                float microPrice = close + RandomGaussian(0, volatility * close * 0.1f);
                high = Mathf.Max(high, microPrice);
                low = Mathf.Min(low, microPrice);
            }

            GameObject newCandle = Instantiate(candlePrefab, candleParent);
            RectTransform candleRect = newCandle.GetComponent<RectTransform>();

            candleRect.anchorMin = new Vector2(0, 0);
            candleRect.anchorMax = new Vector2(0, 0);
            candleRect.pivot = new Vector2(0.5f, 0);

            float xPos = i * candleSpacing + candleSpacing / 2;
            candleRect.anchoredPosition = new Vector2(xPos, 0);

            yield return StartCoroutine(AnimateCandle(newCandle, open, close, high, low, parentHeight));

            if (i == GameConfig.Instance.CandlesPerCycle - 1)
            {
                prices.Add(close);
                playerData.StockPrice = close;

                playerData.StockPriceHistory.Add(close);
                if (playerData.StockPriceHistory.Count > GameConfig.Instance.MaxPriceHistory)
                {
                    playerData.StockPriceHistory.RemoveAt(0);
                }

                if (Random.value < GameConfig.Instance.TrendChangeProbability)
                {
                    isBullishTrend = !isBullishTrend;
                    playerData.IsBullishTrend = isBullishTrend;
                    Debug.Log(isBullishTrend ? "Ринок перейшов у бичачий тренд!" : "Ринок перейшов у ведмежий тренд!");
                }

                GameManager.Instance.NotifyPlayerDataUpdated();
            }

            yield return new WaitForSeconds(GameConfig.Instance.CandleDelay);
        }

        onGenerationStateChanged?.Invoke(false);
    }
    #endregion

    #region Candle Animation
    private IEnumerator AnimateCandle(GameObject candle, float open, float close, float high, float low, float parentHeight)
    {
        RectTransform body = candle.transform.Find("Body").GetComponent<RectTransform>();
        RectTransform wick = candle.transform.Find("Wick").GetComponent<RectTransform>();
        Image bodyImage = body.GetComponent<Image>();
        Image wickImage = wick.GetComponent<Image>();

        body.anchorMin = new Vector2(0.5f, 0);
        body.anchorMax = new Vector2(0.5f, 0);
        body.pivot = new Vector2(0.5f, 0);

        wick.anchorMin = new Vector2(0.5f, 0);
        wick.anchorMax = new Vector2(0.5f, 0);
        wick.pivot = new Vector2(0.5f, 0);

        float usableHeight = parentHeight * (1f - 2f * GameConfig.Instance.CandlePadding);
        float heightOffset = parentHeight * GameConfig.Instance.CandlePadding;

        float normalizedOpen = NormalizePrice(open);
        float normalizedClose = NormalizePrice(close);
        float normalizedHigh = NormalizePrice(high);
        float normalizedLow = NormalizePrice(low);

        float targetBodyHeight = Mathf.Abs(normalizedOpen - normalizedClose) * usableHeight;
        float targetWickHeight = (normalizedHigh - normalizedLow) * usableHeight;
        float bodyY = heightOffset + (normalizedOpen + normalizedClose) / 2 * usableHeight;
        float wickY = heightOffset + (normalizedHigh + normalizedLow) / 2 * usableHeight;

        bodyImage.color = close >= open ? Color.green : Color.red;
        wickImage.color = close >= open ? Color.green : Color.red;

        body.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.7f, 0);
        wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, 0);
        body.anchoredPosition = new Vector2(0, heightOffset + normalizedOpen * usableHeight);
        wick.anchoredPosition = new Vector2(0, heightOffset + normalizedOpen * usableHeight);

        float wickAnimationDuration = GameConfig.Instance.CandleAnimationDuration * 0.3f;
        float elapsedTime = 0f;
        AnimationCurve wickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        while (elapsedTime < wickAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = wickCurve.Evaluate(elapsedTime / wickAnimationDuration);

            float currentWickHeight = Mathf.Lerp(0, targetWickHeight, t);
            wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, currentWickHeight);
            wick.anchoredPosition = new Vector2(0, heightOffset + normalizedLow * usableHeight + currentWickHeight / 2);

            yield return null;
        }

        wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, targetWickHeight);
        wick.anchoredPosition = new Vector2(0, wickY);

        float bodyAnimationDuration = GameConfig.Instance.CandleAnimationDuration * 0.7f;
        elapsedTime = 0f;
        AnimationCurve bodyCurve = AnimationCurve.Linear(0, 0, 1, 1);
        float startHeight = heightOffset + normalizedOpen * usableHeight;
        float targetHeight = heightOffset + normalizedClose * usableHeight;
        float oscillationAmplitude = 10f;

        while (elapsedTime < bodyAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = bodyCurve.Evaluate(elapsedTime / bodyAnimationDuration);

            float currentHeight = Mathf.Lerp(startHeight, targetHeight, t);
            float oscillation = Mathf.Sin(elapsedTime * 15f) * oscillationAmplitude * (1 - t);
            currentHeight += oscillation;

            float currentBodyHeight = Mathf.Abs(currentHeight - startHeight);
            body.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.7f, currentBodyHeight);

            if (close >= open)
            {
                body.anchoredPosition = new Vector2(0, startHeight + currentBodyHeight / 2);
            }
            else
            {
                body.anchoredPosition = new Vector2(0, startHeight - currentBodyHeight / 2);
            }

            float currentNormalizedHeight = (currentHeight - heightOffset) / usableHeight;
            if (currentNormalizedHeight > normalizedHigh)
            {
                normalizedHigh = currentNormalizedHeight;
                targetWickHeight = (normalizedHigh - normalizedLow) * usableHeight;
                wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, targetWickHeight);
                wick.anchoredPosition = new Vector2(0, heightOffset + (normalizedHigh + normalizedLow) / 2 * usableHeight);
            }
            else if (currentNormalizedHeight < normalizedLow)
            {
                normalizedLow = currentNormalizedHeight;
                targetWickHeight = (normalizedHigh - normalizedLow) * usableHeight;
                wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, targetWickHeight);
                wick.anchoredPosition = new Vector2(0, heightOffset + (normalizedHigh + normalizedLow) / 2 * usableHeight);
            }

            yield return null;
        }

        body.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.7f, targetBodyHeight);
        body.anchoredPosition = new Vector2(0, bodyY);
        wick.sizeDelta = new Vector2(GameConfig.Instance.CandleWidth * 0.2f, targetWickHeight);
        wick.anchoredPosition = new Vector2(0, wickY);
    }
    #endregion

    #region Utility Methods
    private float NormalizePrice(float price)
    {
        if (maxPrice == minPrice) return 0.5f;
        return (price - minPrice) / (maxPrice - minPrice);
    }

    public float GetLastPrice() => prices[prices.Count - 1];

    // Генерує випадкову величину з нормальним розподілом (метод Бокса-Мюллера)
    private float RandomGaussian(float mean, float stdDev)
    {
        float u1 = Random.value;
        float u2 = Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        return mean + stdDev * randStdNormal;
    }
    #endregion
}