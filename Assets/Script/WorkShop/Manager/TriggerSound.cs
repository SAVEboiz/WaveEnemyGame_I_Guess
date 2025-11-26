using UnityEngine;

public class TriggerSound : MonoBehaviour
{
    public AudioClip sounds;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            SoundManager.instance.PlaySFX(sounds);
        }
    }
}
