//using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem { 

    public static void SaveGame(SaveData saveData) {

        string path = Application.persistentDataPath + "/saves/Save" + GameData.currentSave + "/";

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "Level" + GameData.level + ".umu", FileMode.Create);

        formatter.Serialize(stream, saveData);
        stream.Close();

        Debug.Log("Saved Game to " + path);
    }

    public static SaveData LoadGame() {

        string path = Application.persistentDataPath + "/saves/Save" + GameData.currentSave + "/";

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path + "Level" + GameData.level + ".umu", FileMode.Open);

        SaveData saveData = formatter.Deserialize(stream) as SaveData;
        stream.Close();

        return saveData;

    }
}