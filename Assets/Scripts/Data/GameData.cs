//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

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

}
