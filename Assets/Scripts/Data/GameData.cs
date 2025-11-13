//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using System.IO;

// This'll be a static class to allow us to easily transfer data across multiple scripts
public static class GameData {

    // Chunk data
    public static readonly int ChunkWidth = 16;

    // Generation data
    public static readonly float WorldScale = 0.1f;

    // World data
    public static readonly int ViewDistance = 4;
    public static readonly int WorldSize = 6000;

    // Cross-Scene variables
    public static int level = 0;
    public static int currentSave = 1;
    
    // Settings
    public static bool debugMode = true;

    // Tests
    public static bool hasSaves {

        get {
            // Im could not be fucked with optimizing this piece of code
            // If it ain't broke, dont fix it!
            return Directory.Exists(Application.persistentDataPath + "/saves/Save1/") || Directory.Exists(Application.persistentDataPath + "/saves/Save2/") || Directory.Exists(Application.persistentDataPath + "/saves/Save3/") || Directory.Exists(Application.persistentDataPath + "/saves/Save4/") || Directory.Exists(Application.persistentDataPath + "/saves/Save5/");
        }

    }

}
