using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{


    public void StartGame()
    {
        SceneManager.LoadScene("mainGameLoop");
    }

    public void InformationScene()
    {
        SceneManager.LoadScene("InformationScene");
    }

    public void OptionsScene()
    {
        SceneManager.LoadScene("OptionsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
