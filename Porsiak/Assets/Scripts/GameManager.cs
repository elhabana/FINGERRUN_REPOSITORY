using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject GameOverPanel;
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private float initialScrollSpeed = 8f;

    [SerializeField] private RankingManager rankingManager;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip gameOverMusic;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip jumpSound;

    public static GameManager Instance { get; private set; }

    private int score;
    private float scoreTimer;
    private float scroollSpeed;
    private bool gameStarted = false;

    public bool IsGameStarted => gameStarted;

    void Update()
    {
        if (gameStarted)
        {
            UpdateScore();
            UpdateSpeed();

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuSystem.Instance != null)
            {
                // Si las opciones están abiertas, llamamos a la función corregida
                if (MenuSystem.Instance.PanelOptions.activeSelf)
                {
                    MenuSystem.Instance.CloseOptions();
                }
                // Solo permitimos abrir la pausa si el juego empezó y NO estamos muertos
                else if (gameStarted && !MenuSystem.Instance.PanelDeadMenu.activeSelf && !MenuSystem.Instance.PanelRanking.activeSelf)
                {
                    MenuSystem.Instance.TogglePause();
                }
            }
        }
    }


    public void StartScoring()
    {
        gameStarted = true;

       
        if (ScoreText != null)
        {
            ScoreText.gameObject.SetActive(true);
        }

   
        if (musicSource != null && backgroundMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
          
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonSound()
    {
        if (musicSource != null && buttonClickSound != null)
        {
            
            musicSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PlayJumpSound()
    {
        if (musicSource != null && jumpSound != null)
        {
            
            musicSource.PlayOneShot(jumpSound);
        }
    }

    void Start()
    {
        if (musicSource != null && menuMusic != null)
        {
            musicSource.clip = menuMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void ShowGameOverPanel()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);

            // OCULTAR LA PUNTUACIÓN AL MORIR
            if (ScoreText != null)
            {
                ScoreText.gameObject.SetActive(false);
            }

            if (rankingManager != null)
            {
                rankingManager.SaveScore(score);
            }

        }

        if (musicSource != null && gameOverMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = gameOverMusic;
            musicSource.loop = false;
            musicSource.Play();
        }
    }

    private void UpdateScore()
    {
        int scoreperSecond = 10;

        scoreTimer += Time.deltaTime;
        score = (int)(scoreTimer * scoreperSecond);
        ScoreText.text = string.Format("{0:00000}", score);
    }

    public float GetScrollSpeed()
    {
        return scroollSpeed;
    }

    private void UpdateSpeed()
    {
        float speeddivider = 10f;
        scroollSpeed = initialScrollSpeed + scoreTimer / speeddivider;
    }
}