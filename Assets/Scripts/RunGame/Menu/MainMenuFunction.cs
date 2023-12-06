using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuFunction : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("RunGame");
    }

    public void GameExplain()
    {
        SceneManager.LoadScene("RunGameExplain");
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Home");
    }

    public void QuitExplain()
    {
        SceneManager.LoadScene("RunMenu");
    }
}
