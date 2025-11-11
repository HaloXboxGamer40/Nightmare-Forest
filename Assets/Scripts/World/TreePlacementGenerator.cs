//using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// We will get a random postion within our chunk
// we will calculate where we need to be inside the chunk
// amd make sure we're not already on top of a diffrent tree
public class TreePlacementGenerator : MonoBehaviour {

    // Public variables, set by World when we initialize this component
    public Chunk chunk;
    public int height;
    public byte type;

    public void PlaceTree(ChunkData chunkData) {

        // Random position from -8 to 8 (chunks centers are 0, 0 so -8 would be on the chunk boarder)
        int x = Random.Range(-GameData.ChunkWidth / 2, GameData.ChunkWidth / 2);
        int z = Random.Range(-GameData.ChunkWidth / 2, GameData.ChunkWidth / 2);

        // Make sure we're not creating a tree thats already at this position in the chunk
        if (!chunk.plants.Contains(new Vector2Int(x, z))) {
            
            // Calculate exactly where we need to be and where the actual chunk is
            Vector3Int position = new Vector3Int(x + (chunk.chunkCenter.x + GameData.ChunkWidth / 2), height / 2, z + (chunk.chunkCenter.y + GameData.ChunkWidth / 2));
            transform.position = position;

            // Add our position to the list and name ourselves
            chunk.plants.Add(new Vector2Int(x, z));
            gameObject.name = name + ": (" + position.x + ", " + position.y + ")";

            PlantData plant = new PlantData(position.x, position.z, type);
            chunkData.AddPlant(plant);

            // Destroy this component since we don't need it anymore
            Destroy(this);

        } else {
            
            // If we already are trying to generate over a tree
            // we will simply destroy ourselves so we don't show up when we activate the chunk again
            Destroy(gameObject);

        }
    }
}