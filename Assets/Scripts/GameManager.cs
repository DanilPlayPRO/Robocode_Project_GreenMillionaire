using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Dependencies")]
    [Tooltip("Менеджер UI для відображення даних")]
    [SerializeField] private UIManager uiManager;
    #endregion

    #region Private Fields
    private UpgradeSystem upgradeSystem;
    private PlayerData playerData;
    private const float PassiveIncomeInterval = 1f;
    public event Action<PlayerData> OnPlayerDataUpdated;
    #endregion

    #region Unity Lifecycle Methods
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        

        playerData = new PlayerData();
        upgradeSystem = new UpgradeSystem();
    }

    private void Start()
    {
        // Ініціалізуємо PlayerData після того, як GameConfig буде готовий
        playerData.Initialize();

        SaveSystem.LoadData(playerData);

        if (uiManager != null)
        {
            uiManager.Initialize(this, upgradeSystem);
        }

        if (upgradeSystem == null)
        {
            Debug.LogError("upgradeSystem не створений. Створюємо новий.");
            upgradeSystem = new UpgradeSystem();
        }
        upgradeSystem.Initialize(playerData, () => OnPlayerDataUpdated?.Invoke(playerData));
        InvokeRepeating(nameof(AddPassiveIncome), PassiveIncomeInterval, PassiveIncomeInterval);
    }

    private void OnLevelWasLoaded(int level)
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.Initialize(this, upgradeSystem);
        }
        else
        {
            Debug.LogWarning("UIManager не знайдений у новій сцені.");
        }
    }
    #endregion

    #region Game Logic
    public void HandleClick()
    {
        if (upgradeSystem == null)
        {
            Debug.LogError("UpgradeSystem не ініціалізований.");
            return;
        }

        double clickValue = playerData.ClickValue * upgradeSystem.GetClickMultiplier;
        playerData.AddBalance(clickValue);
        UpdateAndSave();
    }

    private void AddPassiveIncome()
    {
        playerData.AddBalance(playerData.PassiveIncome);
        UpdateAndSave();
    }

    private void UpdateAndSave()
    {
        OnPlayerDataUpdated?.Invoke(playerData);
        SaveSystem.SaveData(playerData);
    }
    #endregion

    #region Public Methods
    public PlayerData GetPlayerData() => playerData;

    public void NotifyPlayerDataUpdated()
    {
        OnPlayerDataUpdated?.Invoke(playerData);
    }
    #endregion
}