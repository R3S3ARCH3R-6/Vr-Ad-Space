using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu_Scene_Changer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //The following code is used to allow the mouse to appear only on certain screens
        Cursor.visible = true;
            //the statement above allows the mouse to be seen
        Cursor.lockState = CursorLockMode.None;
            //the above statment stops the mouse from being locked into place or disabled
    }

    /// <summary>
    /// Quits the game entirely. This script stops the program/game.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// This function brings the user to the scene "Livingroom"
    /// </summary>
    public void LivingRoom()
    {
        SceneManager.LoadScene("Livingroom");
    }

    /// <summary>
    /// This function brings the user to the scene "Kitchen"
    /// </summary>
    public void Kitchen()
    {
        SceneManager.LoadScene("Kitchen");
    }


    public void Original_Background_Alt()
    {
        SceneManager.LoadScene("Original Background Alt");
    }

    public void Bedroom()
    {
        SceneManager.LoadScene("Bedroom");
    }
}
