using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour {

    public BackgroundManager bm;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject savesMenu;
    public GameObject assuranceMenu;
    public GameObject levelMenu;

    [Header("Saves")]
    public GameObject[] saveObjs;

    [Header("Colors")]
    public Color normalColor;
    public Color notSavedColor;
    public Color finishedColor;

    public bool canClick;
    private bool newGame;

    private int saveToLoad;
    private Saves saves;

    public void LoadMenu(bool newGame) {

        canClick = true;
        this.newGame = newGame;

        saves = new Saves();

        int count = 0;
        foreach (bool saved in saves.saved) {

            Image img = saveObjs[count].GetComponent<Image>();

            if (saved)
                img.color = normalColor;
            else 
                img.color = notSavedColor;

            count++;
        }
    }

    public void Close() {

        if (!canClick)
            return;

        mainMenu.SetActive(true);
        savesMenu.SetActive(false);

    }

    public void SaveSelected(int saveID) {

        if (!canClick)
            return;

        saveToLoad = saveID;
        if(saves.saved[saveID - 1] && newGame) {
            assuranceMenu.SetActive(true);
            canClick = false;
        } else {
            LoadSave();
        }

    }

    public void CloseAssuranceMenu() {

        assuranceMenu.SetActive(false);
        canClick = true;

    }

    public void LoadSave() {

        if (newGame && saves.saved[saveToLoad - 1])
            Directory.Delete(Application.persistentDataPath + "/saves/Save" + saveToLoad + "/", true);

        GameData.currentSave = saveToLoad;

        bm.SetBackground(1, true, assuranceMenu, levelMenu, mainMenu, savesMenu);

        LevelMenu lm = this.GetComponent<LevelMenu>();

        lm.RefreshLevels();

    }

}

public class Saves {

    public bool[] saved = new bool[5];

    public Saves() {

        for (int i = 0; i < 5; i++) {
            saved[i] = Directory.Exists(Application.persistentDataPath + "/saves/Save" + (i + 1) + "/");
        }

    }

}