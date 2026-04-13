using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    
    public TextMeshProUGUI coinText; 
    public int coinCount {private set; get;}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
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
        coinText = FindFirstObjectByType<TextMeshProUGUI>();
        
        if (scene.name == "Level 1")
        {
            ResetCoinCount();
            UpdateCoinUI();
        }
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }
    
    public void ResetCoinCount()
    {
        coinCount = 0;
        UpdateCoinUI();
    }
    
    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = ": " + coinCount + "/7";
    }
}