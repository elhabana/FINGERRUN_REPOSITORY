using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuSystem : MonoBehaviour
{
    [SerializeField] public GameObject PanelOptions;
    [SerializeField] public GameObject PanelMainMenu;
    [SerializeField] public GameObject PanelDeadMenu;
    [SerializeField] public GameObject PanelRanking;

    private GameObject previousRankingPanel;

    private GameObject previousPanel;
    private static bool comesFromRestart = false;

    public static MenuSystem Instance { get; private set; }


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetObstaclesTrigger(bool trigger)
    {
        // Encuentra todos los objetos activos con tag "Obstacle"
        GameObject[] allObstacles = GameObject.FindGameObjectsWithTag("Obstacle");


        foreach (GameObject obj in allObstacles)
        {
            Collider2D col = obj.GetComponent<Collider2D>();
            if (col != null)
                col.isTrigger = trigger;
        }
    }

    void Start()
    {
        if (comesFromRestart)
        {
            Play();
            comesFromRestart = false;
        }
        else
        {
            PanelMainMenu.SetActive(true);
            Time.timeScale = 1f; // El tiempo corre para que el fondo se mueva
            SetObstaclesTrigger(true); // Pero los obstáculos no te matan en el menú
        }
    }

    public void Play()
    {
        PanelMainMenu.SetActive(false);
        SetObstaclesTrigger(false);
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartScoring();
        }
    }

    public void Exit()
    {
        Application.Quit();
        Debug.Log("You closed the game");
    }

    public void PrincipalMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OpenOptionsFrom(GameObject fromPanel)
    {
        previousPanel = fromPanel;
        fromPanel.SetActive(false);
        PanelOptions.SetActive(true);
        SetObstaclesTrigger(true);
    }

    public void CloseOptions()
    {
        PanelOptions.SetActive(false);

        
        if (previousPanel != null)
        {
            previousPanel.SetActive(true);
            
        }

       
        if (GameManager.Instance != null && GameManager.Instance.IsGameStarted)
        {
            
            Time.timeScale = 1f;
            SetObstaclesTrigger(false);
        }
        else
        {
            
            Time.timeScale = 1f;
            SetObstaclesTrigger(true);
        }

        
        previousPanel = null;
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        SetObstaclesTrigger(false);
        PanelMainMenu.SetActive(false);
        Time.timeScale = 1f;

    }

    public void OpenRankingFrom(GameObject fromPanel)
    {
        previousRankingPanel = fromPanel; 
        fromPanel.SetActive(false);       

        if (PanelRanking != null)
        {
            PanelRanking.SetActive(true);
            RankingManager rm = PanelRanking.GetComponent<RankingManager>();
            if (rm != null) rm.UpdateUI(rm.GetScores());
        }
    }

    public void CloseRanking()
    {
        if (PanelRanking != null)
        {
            PanelRanking.SetActive(false); // Cerramos el ranking
        }

        if (previousRankingPanel != null)
        {
            previousRankingPanel.SetActive(true); // Reactivamos el menú de donde vinimos
        }
    }

    public void TogglePause()
    {
        // Si el ranking o el menú muerto están abiertos, no pausamos
        if (PanelDeadMenu.activeSelf || PanelRanking.activeSelf) return;

        if (!PanelOptions.activeSelf)
        {
            // Abrir pausa
            PanelOptions.SetActive(true);
            Time.timeScale = 0f;
            SetObstaclesTrigger(true);
        }
        else
        {
            // Quitar pausa
            CloseOptions();
            Time.timeScale = 1f;
            SetObstaclesTrigger(false);
        }
    }
}
