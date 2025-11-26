using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// กำหนดให้เป็น sealed เพื่อป้องกันการสืบทอด
public class GameManager : MonoBehaviour
{
    // 1. Private Static Field (The Singleton Instance)
    // ใช้ backing field เพื่อควบคุมการเข้าถึง
    public static GameManager instance;
    // 2. Public Static Property (Global Access Point)
   
    [Header("Game State")]
    public int currentScore = 0;
    public bool isGamePaused = false;

    [Header("UI Game")]
    public GameObject canvas;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject winUI;
    public TMP_Text scoreText;
    public Slider HPBar;

    // 3. Private Constructor Logic (ใช้ Awake() แทน Constructor ปกติใน Unity)
    private void Awake()
    {
        // ตรวจสอบว่ามี Instance อยู่แล้วหรือไม่
       if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
       else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // ซ่อน Game Over UI ตอนเริ่มเกม
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    // ------------------- Singleton Functionality -------------------

    public void UpdateHealthBar(int currentHealth, int maxHealth)
    {
       HPBar.value = currentHealth;
       HPBar.maxValue = maxHealth;
    }
    public void AddScore(int amount)
    {
        currentScore += amount;
        scoreText.text = currentScore.ToString();
    }

    public void ShowGameWin()
    {
        if (winUI != null)
        {
            winUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void ShowGameOver()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            Time.timeScale = 0;
        }
    }

    //Restart
    public void OnRestartGameClick()
    {
        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        Debug.Log("Press");
        SceneManager.LoadScene(1);
        Time.timeScale = 1;
    }

    //Main Menu
    public void OnLoadMainMenuClick()
    {
        canvas.SetActive(false);
        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        Debug.Log("Press");
        SceneManager.LoadScene(0); //(Main Menu)
        Time.timeScale = 1;
    }

    public void TogglePause()
    {
       isGamePaused = !isGamePaused;
       Time.timeScale = isGamePaused ? 0 : 1;
        pauseMenuUI.SetActive(isGamePaused);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
}