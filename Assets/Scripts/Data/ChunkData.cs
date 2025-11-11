using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChunkData {

    public int x;
    public int z;

    public List<PlantData> plants = new List<PlantData>();

    public ChunkData(int x, int z) {

        this.x = x;
        this.z = z;

    }

    public void AddPlant(PlantData plant) {

        plants.Add(plant);

    }

}
