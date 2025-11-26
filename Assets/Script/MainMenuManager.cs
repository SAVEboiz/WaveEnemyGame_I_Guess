using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnPlayClick()
    {
        SceneManager.LoadScene(1);
        GameManager.instance.canvas.SetActive(true);
        WaveEnemyUI.instance.UpdateWaveDisplay(1);
    }
    public void OnQuitClick()
    {
        Application.Quit();
    }
}
