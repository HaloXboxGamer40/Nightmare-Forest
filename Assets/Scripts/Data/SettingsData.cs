using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData {

    // Debug
    public bool debugMode;

    // Performance
    public TextureQuality textures;
    public int currentResolution;
    public bool fog;

    public SettingsData() {

        debugMode = false;

        textures = TextureQuality.Medium;
        currentResolution = 3;
        fog = true;

    }

}

public enum TextureQuality {

    Ultra, // 2048
    High, // 1024
    Medium, // 512
    Low, // 256
    Potato // 128

}