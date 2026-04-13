using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Required for Button component
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [HideInInspector] public GameObject winScreenUI;
    [HideInInspector] public GameObject pauseMenuUI;

    private bool isGamePaused;
    public bool hasKey;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("GameManager successfully loaded scene: " + scene.name);
        GameObject canvas = GameObject.FindGameObjectWithTag("UICanvas");

        if (canvas == null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (Canvas canvasComponent in canvases)
            {
                if (canvasComponent.name == "UICanvas")
                {
                    canvas = canvasComponent.gameObject;
                    break;
                }
            }
        }

        if (canvas != null)
        {
            // find and cache the UI panels
            Transform pauseMenuTransform = canvas.transform.Find("PauseMenu");
            Transform winScreenTransform = canvas.transform.Find("WinScreenUI");

            pauseMenuUI = pauseMenuTransform != null ? pauseMenuTransform.gameObject : null;
            winScreenUI = winScreenTransform != null ? winScreenTransform.gameObject : null;

            // set initial state and link buttons
            if (pauseMenuUI != null) 
            {
                pauseMenuUI.SetActive(false);
                LinkPauseMenuButtons(); // only links Resume and MainMenu
            }
            if (winScreenUI != null) 
            {
                winScreenUI.SetActive(false);
                LinkWinMenuButtons();
            }
        }
        
        // reset game state on scene load
        Time.timeScale = 1f;
        isGamePaused = false;
        hasKey = false;
        if (AudioManager.instance != null)
        {
             AudioManager.instance.MuteAllAudio(false); 
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && pauseMenuUI != null) 
        {
            Debug.Log("Escape key pressed!");
            if (pauseMenuUI == null) return;
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void HandleGameWin()
    {
        if (winScreenUI != null)
        {
            Time.timeScale = 0f;
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopMusic();
                AudioManager.instance.ResetMusicPitch();
            }
            winScreenUI.SetActive(true);
        }
        else
        {
             Debug.LogError("Win Screen UI is not linked! Game is frozen but Win Screen not shown.");
        }
    }
    public void PauseGame()
    {
        if (winScreenUI != null && winScreenUI.activeSelf) return; 
        if (pauseMenuUI == null) return; 

        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        if (AudioManager.instance != null)
        {
            AudioManager.instance.MuteAllAudio(true);
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI == null) return; 
        
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isGamePaused = false;
        if (AudioManager.instance != null)
        {
            AudioManager.instance.MuteAllAudio(false);
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    // used when changing scenes, so the buttons stay
    private void LinkPauseMenuButtons()
    {
        Button resumeButton = pauseMenuUI.transform.Find("ResumeButton")?.GetComponent<Button>();
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame); 
        }
        Button mainMenuButton = pauseMenuUI.transform.Find("MenuButton")?.GetComponent<Button>();
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu); 
        }
    }
    
    private void LinkWinMenuButtons()
    {
        Button restartButton = winScreenUI.transform.Find("RestartButton")?.GetComponent<Button>();
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartLevel); 
        }
        Button mainMenuButton = winScreenUI.transform.Find("MenuButton")?.GetComponent<Button>();
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu); 
        }
        Button quitButton = winScreenUI.transform.Find("QuitButton")?.GetComponent<Button>();
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame); 
        }
    }
}
