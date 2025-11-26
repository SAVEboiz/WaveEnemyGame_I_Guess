using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public List<WaveEnemyData> waves = new List<WaveEnemyData>();
    public Transform[] spawnPoints;

    [Header("Settings")]
    public bool autoStartWaves = true;
    public float delayBeforeFirstWave = 3f;

    [Header("Debug Info")]
    public int currentWaveIndex = 0;
    public int enemiesRemain = 0;
    public bool waveInProgress = false;

    public static WaveEnemyManager Instance;

    // Events
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveStart;
    public event WaveEvent OnWaveComplete;
    public event System.Action OnAllWavesComplete;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to WaveManager!");
            return;
        }

        if (autoStartWaves && waves.Count > 0)
        {
            StartCoroutine(StartWaveSequence());
        }
    }

    IEnumerator StartWaveSequence()
    {
        yield return new WaitForSeconds(delayBeforeFirstWave);

        while (currentWaveIndex < waves.Count)
        {
            yield return StartCoroutine(SpawnWave(currentWaveIndex));
            currentWaveIndex++;
        }

        OnAllWavesComplete?.Invoke();
        Debug.Log("All waves complete!");
        GameManager.instance.ShowGameWin();
    }

    IEnumerator SpawnWave(int waveIndex)
    {
        if (waveIndex >= waves.Count) yield break;

        WaveEnemyData wave = waves[waveIndex];
        waveInProgress = true;

        Debug.Log($"Starting Wave {wave.waveNum}: Spawning {wave.enemyCount} enemies");
        OnWaveStart?.Invoke(wave.waveNum);

        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyPrefabs);
            if (i < wave.enemyCount - 1)
            {
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        yield return new WaitUntil(() => enemiesRemain == 0);

        Debug.Log($"Wave {wave.waveNum} Complete!");
        OnWaveComplete?.Invoke(wave.waveNum);
        waveInProgress = false;

        if (waveIndex < waves.Count - 1)
        {
            Debug.Log($"Resting for {wave.restTime} seconds before next wave...");
            yield return new WaitForSeconds(wave.restTime);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab is null!");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        spawnedEnemies.Add(enemy);
        enemiesRemain++;


        Enemy enemyScript = enemy.GetComponent<Enemy>();
        if (enemyScript != null)
        {
            enemyScript.OnDeath += () => OnEnemyDeath(enemy);
        }
    }

    public void OnEnemyDeath(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
            enemiesRemain--;
            enemiesRemain = Mathf.Max(0, enemiesRemain);
        }
    }

    // Manual controls
    public void StartNextWave()
    {
        if (!waveInProgress && currentWaveIndex < waves.Count)
        {
            StartCoroutine(SpawnWave(currentWaveIndex));
            currentWaveIndex++;
        }
    }

    public void ResetWaves()
    {
        StopAllCoroutines();

        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();
        enemiesRemain = 0;
        currentWaveIndex = 0;
        waveInProgress = false;
    }
}