using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void quitGame() // Renamed for clarity, but you can keep the old name if you want.
    {
        // Instead of quitting the application, load the "Main Menu" scene.
        SceneManager.LoadScene("Main Menu");
    }

    public void paused()
    {
        Time.timeScale = 0;
    }

    public void resume()
    {
        Time.timeScale = 1;
    }
}