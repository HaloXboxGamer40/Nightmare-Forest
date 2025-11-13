using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SettingsMenu : MonoBehaviour {

    [Header("Background Manager")]
    public BackgroundManager bm;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject settingsMenu;

    [Header("Settings")]
    public Toggle debugToggle;
    public CycleButton textureQuality;
    public CycleButton resolutionsCycle;
    public Toggle fogToggle;

    [Header("Textures")]
    public TextureSizes[] textures;

    [Header("Materials")]
    public Material forestFloor;

    [HideInInspector]
    public SettingsData settings;
    public string path;

    Resolution[] resolutions = new Resolution[4];

    private void Awake() {

        AddResolutions();

        resolutionsCycle.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
            resolutionsCycle.AddOptions(resolutions[i].width + "x" + resolutions[i].height);
        

        path = Application.persistentDataPath;

        if (!File.Exists(path + "/settings.json")) {

            settings = new SettingsData();
            string jsonExport = JsonUtility.ToJson(settings);
            File.WriteAllText(path + "/settings.json", jsonExport);

        } else {

            string jsonImport = File.ReadAllText(path + "/settings.json");
            settings = JsonUtility.FromJson<SettingsData>(jsonImport);

        }
    }

    public void OpenSettings() {

        debugToggle.isOn = settings.debugMode;
        textureQuality.selectedOption = (int)settings.textures;
        resolutionsCycle.selectedOption = settings.currentResolution;
        fogToggle.isOn = settings.fog;

    }

    public void CloseSettings() {

        settings.debugMode = debugToggle.isOn;
        settings.textures = (TextureQuality)textureQuality.selectedOption;
        settings.currentResolution = resolutionsCycle.selectedOption;
        settings.fog = fogToggle.isOn;

        UpdateTextures(textureQuality.selectedOption);
        UpdateResolution();

        RenderSettings.fog = settings.fog;

        string jsonExport = JsonUtility.ToJson(settings);
        File.WriteAllText(path + "/settings.json", jsonExport);

        bm.SetBackground(0, true, settingsMenu, mainMenu);

    }

    private void UpdateTextures(int qualityLevel) {

        forestFloor.SetTexture("_MainTex", textures[1].ReturnTexture(qualityLevel));
        forestFloor.SetTexture("_MetallicGlossMap", textures[3].ReturnTexture(qualityLevel));
        //forestFloor.SetTexture("_BumpMap", textures[1].ReturnTexture(qualityLevel));
        forestFloor.SetTexture("_ParallaxMap", textures[2].ReturnTexture(qualityLevel));
        forestFloor.SetTexture("_OcclusionMap", textures[0].ReturnTexture(qualityLevel));

    }

    public void UpdateResolution() {

        SetResolution(resolutionsCycle.selectedOption);

    }

    public void SetResolution (int resolutionIndex) {

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

    }

    private void AddResolutions() {
        Resolution[] allRes = Screen.resolutions;

        // remove dups and sort
        Resolution[] uniqueRes = allRes
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .OrderBy(r => r.width)
            .ThenBy(r => r.height)
            .ToArray();

        resolutions = new Resolution[4];

        if (uniqueRes.Length < 4) {
            for (int i = 0; i < 4; i++)
                resolutions[i] = uniqueRes[Mathf.Min(i, uniqueRes.Length - 1)];
            return;
        }

        int step = Mathf.Max(1, uniqueRes.Length / 4);

        for (int i = 0; i < 4; i++) {
            int index = (i == 3) ? (uniqueRes.Length - 1) : (i * step);
            index = Mathf.Clamp(index, 0, uniqueRes.Length - 1);
            resolutions[i] = uniqueRes[index];
        }
    }

}

[System.Serializable]
public class TextureSizes {

    public Texture2D tex2048;
    public Texture2D tex1024;
    public Texture2D tex512;
    public Texture2D tex256;
    public Texture2D tex128;

    public Texture2D ReturnTexture(int id) {

        switch(id) {
            case 4:
                return tex2048;
            case 3:
                return tex1024;
            case 2:
                return tex512;
            case 1:
                return tex256;
            case 0:
                return tex128;
            default:
                return null;
        }

    }

}