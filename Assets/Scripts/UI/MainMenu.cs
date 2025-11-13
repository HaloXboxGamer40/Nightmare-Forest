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
    public GameObject saveMenu;

    public GameObject loadGame;

    [Header("Extras")]
    public GameObject title;

    private void Start() {

        Cursor.lockState = CursorLockMode.None;

        string path = Application.persistentDataPath;

        // If we have any saves then we can feel free to open up the load game button
        if (GameData.hasSaves) {
            loadGame.SetActive(true);
        } else {
            loadGame.SetActive(false);
        }

    }

    /*public void Play() {
        bm.SetBackground(1, true, mainMenu, levelMenu);
    }*/

    public void NewGame() {

        saveMenu.SetActive(true);
        mainMenu.SetActive(false);

        SaveMenu saveMenuScript = this.GetComponent<SaveMenu>();
        saveMenuScript.LoadMenu(true);

    }

    public void LoadGame() {

        saveMenu.SetActive(true);
        mainMenu.SetActive(false);

        SaveMenu saveMenuScript = this.GetComponent<SaveMenu>();
        saveMenuScript.LoadMenu(false);

    }

    public void Settings() {

        bm.SetBackground(2, true, mainMenu, settingsMenu);
        sm.OpenSettings();

    }

    public void Quit() {
        Application.Quit();
    }
}
