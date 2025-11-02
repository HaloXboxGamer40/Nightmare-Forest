using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Background Manager")]
    public BackgroundManager bm;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject levelMenu;

    public void Play() {
        bm.SetBackground(1, true, mainMenu, levelMenu);
    }

    public void Quit() {
        Application.Quit();
    }

    public void Back() {
        bm.SetBackground(0, true, levelMenu, mainMenu);
    }

    public void I() {
        SceneManager.LoadScene("Level 1");
    }
}
