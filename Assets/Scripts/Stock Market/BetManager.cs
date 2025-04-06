using UnityEngine;
using System.Collections;

public class BetManager
{
    #region Fields
    private PlayerData playerData;
    private CandleGenerator candleGenerator;
    private System.Action onBetProcessed;
    #endregion

    #region Initialization
    public void Initialize(PlayerData data, CandleGenerator generator, System.Action onBetProcessedCallback)
    {
        playerData = data;
        candleGenerator = generator;
        onBetProcessed = onBetProcessedCallback;
    }
    #endregion

    #region Betting Logic
    public IEnumerator ProcessBet(bool betOnRise)
    {
        float lastPrice = candleGenerator.GetLastPrice();
        yield return GameManager.Instance.StartCoroutine(candleGenerator.GenerateCandlesGradually(state => { }));
        float newPrice = candleGenerator.GetLastPrice();

        bool win = (betOnRise && newPrice > lastPrice) || (!betOnRise && newPrice < lastPrice);
        double amount = win ? GameConfig.Instance.BetWinAmount : -GameConfig.Instance.BetLossAmount;

        if (win)
        {
            playerData.AddBalance(amount);
            AudioManager.Instance?.PlayWinSound();
            Debug.Log($"Ставка виграла! Отримано: {amount:F2} $");
        }
        else
        {
            playerData.SpendBalance(-amount);
            AudioManager.Instance?.PlayLoseSound();
            Debug.Log($"Ставка програла. Втрачено: {-amount:F2} $");
        }

        GameManager.Instance.NotifyPlayerDataUpdated();
        SaveSystem.SaveData(playerData);

        onBetProcessed?.Invoke();
    }
    #endregion
}