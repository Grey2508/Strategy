using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] Canvas MenuCanvas;

    public void ReloadGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ShowMenu()
    {
        Time.timeScale = 0.1f;

        MenuCanvas.enabled = true;
    }

    public void HideMenu()
    {
        Time.timeScale = 1.0f;

        MenuCanvas.enabled = false;
    }
}
