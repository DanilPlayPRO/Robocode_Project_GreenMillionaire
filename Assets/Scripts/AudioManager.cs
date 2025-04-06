using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton
    public static AudioManager Instance { get; private set; }
    #endregion

    #region Serialized Fields
    [Header("Audio Sources")]
    [Tooltip("Джерело звуку для фонової музики")]
    [SerializeField] private AudioSource backgroundMusicSource;

    [Tooltip("Джерело звуку для звукових ефектів UI")]
    [SerializeField] private AudioSource uiSoundSource;

    [Header("Audio Clips")]
    [Tooltip("Кліп фонової музики")]
    [SerializeField] private AudioClip backgroundMusicClip;

    [Tooltip("Звук для кліку по кнопках")]
    [SerializeField] private AudioClip buttonClickSound;

    [Tooltip("Звук для виграшу ставки")]
    [SerializeField] private AudioClip winSound;

    [Tooltip("Звук для програшу ставки")]
    [SerializeField] private AudioClip loseSound;
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
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }
    #endregion

    #region Audio Control
    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicClip != null)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
        else
        {
            Debug.LogWarning("Не призначений AudioSource або AudioClip для фонової музики.");
        }
    }

    public void PlayButtonClickSound()
    {
        if (uiSoundSource != null && buttonClickSound != null)
        {
            uiSoundSource.PlayOneShot(buttonClickSound);
        }
        else
        {
            Debug.LogWarning("Не призначений AudioSource або AudioClip для звуку кнопки.");
        }
    }

    public void PlayWinSound()
    {
        if (uiSoundSource != null && winSound != null)
        {
            uiSoundSource.PlayOneShot(winSound);
        }
        else
        {
            Debug.LogWarning("Не призначений AudioSource або AudioClip для звуку виграшу.");
        }
    }

    public void PlayLoseSound()
    {
        if (uiSoundSource != null && loseSound != null)
        {
            uiSoundSource.PlayOneShot(loseSound);
        }
        else
        {
            Debug.LogWarning("Не призначений AudioSource або AudioClip для звуку програшу.");
        }
    }
    #endregion
}