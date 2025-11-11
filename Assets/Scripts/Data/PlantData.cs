using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlantData {

    public int x;
    public int z;
    public byte type;

    public PlantData(int x, int z, byte type) {

        this.x = x;
        this.z = z;
        this.type = type;

    }

}
