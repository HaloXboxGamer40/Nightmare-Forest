using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    [Header("World Terrain Values")]
    public Material material;
    public Foliage[] plants;

    [Header("Outside Refrences")]
    public Transform player;

    private Stack<Vector2Int> chunksToCreate = new Stack<Vector2Int>();
    private Dictionary<Vector2Int, Chunk> chunks = new Dictionary<Vector2Int, Chunk>();

    private List<Vector2Int> previouslyActiveChunks = new List<Vector2Int>();
    private List<Vector2Int> activeChunks = new List<Vector2Int>();

    private Vector2Int lastPlayerPos;

    private static World _instance;
    public static World Instance { get { return _instance; } }

    private bool isCreatingChunks;

    // Just making our world script static so we can access it anywhere
    private void Awake() {

        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

    }

    private void Start() {

        GenerateWorld();

    }

    private void Update() {

        // Our "playerPosition" which is actully the chunk our player is in
        Vector2Int playerPosition = new Vector2Int(Mathf.FloorToInt(player.position.x / 16), Mathf.FloorToInt(player.position.z / 16));

        // Check view distance only needs to be called when it must be called
        if(!playerPosition.Equals(lastPlayerPos)) {
            lastPlayerPos = playerPosition;
            CheckViewDistance();
        }

        // My way of avoiding threading
        if (!isCreatingChunks && chunksToCreate.Count > 0)
            StartCoroutine("CreateChunks");

    }

    // This will only get called once in just initialize the world
    public void GenerateWorld() {
        Vector2Int playerPosition = new Vector2Int(Mathf.FloorToInt(player.position.x / 16), Mathf.FloorToInt(player.position.z / 16));

        // the world can be on a 2D grid
        for (int x = playerPosition.x - GameData.ViewDistance; x < playerPosition.x + GameData.ViewDistance; x++) {
            for (int z = playerPosition.y - GameData.ViewDistance; z < playerPosition.y + GameData.ViewDistance; z++) {
                // Using Queues bc they're faster for what we're doing
                chunksToCreate.Push(new Vector2Int(x, z));
                previouslyActiveChunks.Add(new Vector2Int(x, z)); // Also we gotta keep track of this chunk so we can see if we need to unload it later
            }
        }
    }

    // This is the thing that creates/loads/unloads chunks
    private void CheckViewDistance() {

        activeChunks.Clear();

        for (int x = lastPlayerPos.x - GameData.ViewDistance; x < lastPlayerPos.x + GameData.ViewDistance; x++) {
            for (int z = lastPlayerPos.y - GameData.ViewDistance; z < lastPlayerPos.y + GameData.ViewDistance; z++) {
                
                // This is the list of all the chunkPositions that'll be loaded
                activeChunks.Add(new Vector2Int(x, z));

                // This is our chunk loading/unloading checker
                Vector2Int pos = new Vector2Int(x, z);
                // If we have already made this chunk and its not active then make it active 
                // (we dont have to check if we should make it active because we're in this loop)
                if (chunks.ContainsKey(pos)) {
                    // If chunk exists, just make it active if needed
                    if (!chunks[pos].isActive)
                        chunks[pos].isActive = true;
                } else {
                    // Only queue if we don't already plan to create it
                    if (!chunksToCreate.Contains(pos))
                        chunksToCreate.Push(pos);
                }
            }
        }

        // now we're going through our old chunks to see if we need to keep them or not
        foreach (Vector2Int chunkPos in previouslyActiveChunks) {
            // If we don't need their position loaded, we can just unload it
            if (!activeChunks.Contains(chunkPos))
                chunks[chunkPos].isActive = false;
                //chunks[chunkPos].chunk.SetActive(false);
        }

        // Set our old activeChunks list to the previouslyActiveChunks and clear activeChunks.
        previouslyActiveChunks = new List<Vector2Int>(activeChunks);
    }

    IEnumerator CreateChunks() {

        // Locking the creatChunks class
        isCreatingChunks = true;

        // We go through our chunksToCreate Queue and create the chunk with our chunk class,
        // we will only generate one chunk per frame that way we don't slow down the game
        // on loading/generating chunks.
        while (chunksToCreate.Count > 0) {

            Vector2Int chunkPos = chunksToCreate.Pop();
            
            // If we have already made this chunk, then we don't need to worry about it and 
            // we can wait for the next frame.
            if (chunks.ContainsKey(chunkPos)) {
                yield return null;
                continue;
            }

            // Create our chunk object and add it to the list
            Chunk chunk = new Chunk(new Vector2Int(chunkPos.x, chunkPos.y), material);
            chunk.isActive = false; // Set it to false so we don't run any scripts prematurly
            chunks.Add(chunkPos, chunk);

            // This is our tree generator
            foreach (Foliage plant in plants) {
                
                // Get the plants amount so we know how many of THESE typs of trees are going to be in the chunk
                int plantsAmount = Random.Range(0, plant.maxPerChunk);

                // All the stuff we have to do to place a tree
                for (int i = 0; i < plantsAmount; i++) {

                    // Create a new tree at 0, 0, 0 (positional calculations is done later)
                    GameObject plantObj = Instantiate(plant.plantObject, new Vector3Int(0, 0, 0), Quaternion.identity);

                    // Set it as a chunk child, just some inspector stuff
                    plantObj.transform.SetParent(chunk.chunk.transform);

                    // Add our tree placement calculator
                    TreePlacementGenerator tpg = plantObj.AddComponent<TreePlacementGenerator>();

                    // Variables we have to set here for the Tree Placement Generator
                    tpg.chunk = chunk;
                    tpg.height = plant.height;
                    tpg.type = plant.type;
                }
            }

            // Set the chunk to active and wait for the next frame
            chunk.isActive = true;
            yield return null;
        }
        
        // When we're dont creating chunks, we just stop creating them
        isCreatingChunks = false;
    }
}


/*~~~ NOTED BUGS: ~~~*/
// We keep adding the same chunk to chunksToCreate somewhere in GenerateWorld or CheckViewDistance
// Notcing how we try to activate chunks that aren't in the Dictionary