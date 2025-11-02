//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

// Just a simple Scriptable Object script that allows us to store important data for our trees!
[CreateAssetMenu(fileName = "Foliage", menuName = "Data/Foliage")]
public class Foliage : ScriptableObject {

    [Header("Plant Data")]
    public string type;
    public GameObject plantObject;
    public bool hasCollider;
    public int height;

    [Header("Generator Settings")]
    public int maxPerChunk;

}
