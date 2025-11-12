using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour {

    [Header("Refrences")]
    public BackgroundManager bm;
    public SettingsMenu sm;

    [Header("Buttons")]
    public GameObject[] levels;

    [Header("Extra Refrences")]
    public GameObject buttonOverlay;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject levelMenu;

    private void Update() {
        bool hovered = false;

        foreach (GameObject level in levels) {
            if (IsMouseOver(level)) {
                hovered = true;

                // position overlay
                RectTransform levelRT = level.GetComponent<RectTransform>();
                RectTransform overlayRT = buttonOverlay.GetComponent<RectTransform>();
                
                Vector2 position = new Vector2(0, levelRT.anchoredPosition.y);
                overlayRT.anchoredPosition = position;

                break; // found hovered button, no need to continue
            }
        }

        buttonOverlay.SetActive(hovered);
    }

    public void Day_1() {
        ApplySettings();
        SceneManager.LoadScene("Level 1");
    }

    public void Back() {
        bm.SetBackground(0, true, levelMenu, mainMenu);
    }

    public bool IsMouseOver(GameObject obj) {
        PointerEventData pointerData = new PointerEventData(EventSystem.current) {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results) {
            if (result.gameObject == obj)
                return true;
        }

        return false;
    }

    void ApplySettings() {

        GameData.debugMode = sm.settings.debugMode;

    }

}
