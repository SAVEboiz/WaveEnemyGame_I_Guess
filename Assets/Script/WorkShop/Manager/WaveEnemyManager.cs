using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class WaveEnemyManager : MonoBehaviour
{
    [Header("Wave Configuration")]
    public List<WaveEnemyData> waves = new List<WaveEnemyData>();
    public Transform[] spawnPoints;

    [Header("Key Item Settings")]
    public GameObject keyItemPrefab; // Prefab ของ Key Item ที่จะดรอป
    public float keyItemDropHeight = 0.5f; // ความสูงที่ดรอป Key Item

    [Header("Settings")]
    public bool autoStartWaves = true;
    public float delayBeforeFirstWave = 3f;

    [Header("Debug Info")]
    public int currentWaveIndex = 0;
    public int enemiesRemain = 0;
    public bool waveInProgress = false;
    public bool waitingForKeyItem = false; // รอเก็บ Key Item

    public static WaveEnemyManager Instance;

    // Events
    public delegate void WaveEvent(int waveNumber);
    public event WaveEvent OnWaveStart;
    public event WaveEvent OnWaveComplete;
    public event System.Action OnAllWavesComplete;
    public event System.Action OnKeyItemDropped; // Event เมื่อดรอป Key Item
    public event System.Action OnKeyItemCollected; // Event เมื่อเก็บ Key Item

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private GameObject lastEnemyInWave = null; // เก็บ reference ของ enemy ตัวสุดท้าย

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
        waitingForKeyItem = false;

        Debug.Log($"Starting Wave {wave.waveNum}: Spawning {wave.enemyCount} enemies");
        OnWaveStart?.Invoke(wave.waveNum);

        // Spawn enemies ทั้งหมดใน wave
        for (int i = 0; i < wave.enemyCount; i++)
        {
            // ตรวจสอบว่าเป็น enemy ตัวสุดท้ายหรือไม่
            bool isLastEnemy = (i == wave.enemyCount - 1);
            SpawnEnemy(wave.enemyPrefabs, isLastEnemy);

            if (i < wave.enemyCount - 1)
            {
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }

        // รอจนกว่า enemy ทั้งหมดจะตาย
        yield return new WaitUntil(() => enemiesRemain == 0);

        // ถ้าไม่ใช่ wave สุดท้าย ให้รอเก็บ Key Item ก่อน
        if (waveIndex < waves.Count - 1)
        {
            Debug.Log($"Wave {wave.waveNum} Complete! Waiting for Key Item to be collected...");

            // รอจนกว่าจะเก็บ Key Item
            yield return new WaitUntil(() => !waitingForKeyItem);

            Debug.Log($"Key Item collected! Resting for {wave.restTime} seconds before next wave...");
            yield return new WaitForSeconds(wave.restTime);
        }
        else
        {
            Debug.Log($"Wave {wave.waveNum} Complete! Final wave cleared!");
        }

        OnWaveComplete?.Invoke(wave.waveNum);
        waveInProgress = false;
    }

    private void SpawnEnemy(GameObject enemyPrefab, bool isLastEnemy = false)
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

        // ถ้าเป็น enemy ตัวสุดท้ายของ wave นี้ ให้เก็บ reference ไว้
        if (isLastEnemy)
        {
            lastEnemyInWave = enemy;
            Debug.Log($"Last enemy of wave {currentWaveIndex + 1} spawned!");
        }

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

            // ตรวจสอบว่าเป็น enemy ตัวสุดท้ายของ wave หรือไม่
            if (enemy == lastEnemyInWave && enemiesRemain == 0)
            {
                Debug.Log($"Last enemy defeated! Dropping Key Item...");
                DropKeyItem(enemy.transform.position);
                lastEnemyInWave = null;
            }
        }
    }

    private void DropKeyItem(Vector3 position)
    {
        // ถ้าเป็น wave สุดท้ายไม่ต้องดรอป Key Item
        if (currentWaveIndex >= waves.Count - 1)
        {
            Debug.Log("Final wave - no Key Item needed.");
            return;
        }

        if (keyItemPrefab == null)
        {
            Debug.LogWarning("Key Item prefab is not assigned in WaveEnemyManager!");
            waitingForKeyItem = false; // ข้ามการรอถ้าไม่มี prefab
            return;
        }

        // สร้าง Key Item ที่ตำแหน่งของ enemy ที่ตาย
        Vector3 dropPosition = position + Vector3.up * keyItemDropHeight;
        GameObject keyItem = Instantiate(keyItemPrefab, dropPosition, Quaternion.identity);

        waitingForKeyItem = true;
        OnKeyItemDropped?.Invoke();

        Debug.Log($"Key Item dropped for Wave {currentWaveIndex + 1}! Collect it to continue.");

        // ตั้งค่า wave number ให้กับ Key Item
        KeyItem keyItemScript = keyItem.GetComponent<KeyItem>();
        if (keyItemScript != null)
        {
            keyItemScript.waveNumber = currentWaveIndex + 1;
        }
        else
        {
            Debug.LogWarning("KeyItem component not found on prefab!");
        }
    }

    // เรียกใช้ method นี้เมื่อผู้เล่นเก็บ Key Item
    public void CollectKeyItem()
    {
        if (waitingForKeyItem)
        {
            waitingForKeyItem = false;
            OnKeyItemCollected?.Invoke();
            Debug.Log($"Key Item for Wave {currentWaveIndex + 1} collected! Next wave incoming...");
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
        waitingForKeyItem = false;
        lastEnemyInWave = null;
    }
}