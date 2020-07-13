using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Changer : MonoBehaviour
{
    public static bool menuCalled = false;
    public GameObject SceneNav;

    // Start is called before the first frame update
    void Start()
    {
        menuCalled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (OVRInput.Get(OVRInput.Button.One)) {
            if (menuCalled) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume() {
        SceneNav.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        menuCalled = false;
    }

    void Pause() {
        SceneNav.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        menuCalled = true;
    }

    /*public void LoadMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }*/
}
