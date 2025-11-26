using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveEnemyUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI enemyCountText;

    [Header("Optional Settings")]
    public string wavePrefix = "Wave: ";
    public string enemyPrefix = "Enemies: ";

    public static WaveEnemyUI instance;

    private void Awake()
    {
        // ตรวจสอบว่ามี Instance อยู่แล้วหรือไม่
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void Start()
    {
        if (WaveEnemyManager.Instance != null)
        {
            // Subscribe to wave events
            WaveEnemyManager.Instance.OnWaveStart += UpdateWaveDisplay;
            WaveEnemyManager.Instance.OnAllWavesComplete += OnAllWavesComplete;
        }

        UpdateUI();
    }

    public void Update()
    {
        // Update enemy count every frame
        if (WaveEnemyManager.Instance != null)
        {
            UpdateEnemyCount();
            UpdateWaveDisplay(WaveEnemyManager.Instance.currentWaveIndex + 1);
        }
    }

    public void UpdateWaveDisplay(int waveNumber)
    {
        if (waveText != null)
        {
            waveText.text = wavePrefix + waveNumber;
        }
    }

    private void UpdateEnemyCount()
    {
        if (enemyCountText != null && WaveEnemyManager.Instance != null)
        {
            enemyCountText.text = enemyPrefix + WaveEnemyManager.Instance.enemiesRemain;
        }
    }

    private void UpdateUI()
    {
        if (WaveEnemyManager.Instance != null)
        {
            // Show current wave
            if (waveText != null)
            {
                int displayWave = WaveEnemyManager.Instance.currentWaveIndex + 1;
                if (displayWave > WaveEnemyManager.Instance.waves.Count)
                {
                    displayWave = WaveEnemyManager.Instance.waves.Count;
                }
                waveText.text = wavePrefix + displayWave;
            }

            // Show enemy count
            UpdateEnemyCount();
        }
    }

    private void OnAllWavesComplete()
    {
        if (waveText != null)
        {
            waveText.text = "All Waves Complete!";
        }
        if (enemyCountText != null)
        {
            enemyCountText.text = enemyPrefix + "0";
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (WaveEnemyManager.Instance != null)
        {
            WaveEnemyManager.Instance.OnWaveStart -= UpdateWaveDisplay;
            WaveEnemyManager.Instance.OnAllWavesComplete -= OnAllWavesComplete;
        }
    }
}
