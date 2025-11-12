using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Class Refrences")]
    public BackgroundManager bm;
    public SettingsMenu sm;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject levelMenu;
    public GameObject settingsMenu;

    [Header("Extras")]
    public GameObject title;

    private void Start() {

        Cursor.lockState = CursorLockMode.None;

    }

    public void Play() {
        bm.SetBackground(1, true, mainMenu, levelMenu);
    }

    public void Settings() {

        bm.SetBackground(2, true, mainMenu, settingsMenu);
        sm.OpenSettings();

    }

    public void Quit() {
        Application.Quit();
    }
}
