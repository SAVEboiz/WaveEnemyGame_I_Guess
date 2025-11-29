using UnityEngine;

public class KeyItem : Item
{
    [Header("Key Item Settings")]
    public int waveNumber = 1; // Wave ที่ Key Item นี้ใช้สำหรับ

    [Header("Effects")]
    public AudioClip collectSound; // เสียงเมื่อเก็บ

    private Vector3 startPosition;
    private AudioSource audioSource;

    public int Value = 20;

    // Override method OnCollect จาก CollectableItem
    public override void OnCollect(Player player)
    {

        // แจ้ง WaveEnemyManager ว่าเก็บ Key Item แล้ว
        if (WaveEnemyManager.Instance != null)
        {
            WaveEnemyManager.Instance.CollectKeyItem();
            Debug.Log($"Key Item for Wave {waveNumber} collected!");
        }
        else
        {
            Debug.LogWarning("WaveEnemyManager.Instance is null!");
        }

        // เล่นเสียงเก็บ
        if (audioSource != null && collectSound != null)
        {
            AudioSource.PlayClipAtPoint(collectSound, transform.position);
        }
        base.OnCollect(player);
        Destroy(gameObject);
    }

    // วาด Gizmos เพื่อดูใน Scene View
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
