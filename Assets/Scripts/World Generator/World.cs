using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    // Refrences to stuff
    public Material material;
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

        for (int x = lastPlayerPos.x - GameData.ViewDistance; x < lastPlayerPos.x + GameData.ViewDistance; x++) {
            for (int z = lastPlayerPos.y - GameData.ViewDistance; z < lastPlayerPos.y + GameData.ViewDistance; z++) {
                
                // This is the list of all the chunkPositions that'll be loaded
                activeChunks.Add(new Vector2Int(x, z));

                // This is our chunk loading/unloading checker
                Vector2Int pos = new Vector2Int(x, z);
                // If we have already made this chunk and its not active then make it active 
                // (we dont have to check if we should make it active because we're in this loop)
                if (chunks.ContainsKey(pos) && !chunks[pos].isActive)
                    chunks[pos].isActive = true;
                    //chunks[pos].chunk.SetActive(true);
                else  // Else add it to the chunks to create list for later
                    chunksToCreate.Push(new Vector2Int(x, z)); 
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
        activeChunks.Clear();
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

            Chunk chunk = new Chunk(new Vector2Int(chunkPos.x, chunkPos.y), material);
            chunks.Add(chunkPos, chunk);
            yield return null;
        }

        isCreatingChunks = false;
    }
}

/*~~~ NOTED BUGS: ~~~*/
// We keep adding the same chunk to chunksToCreate somewhere in GenerateWorld or CheckViewDistance
// Notcing how we try to activate chunks that aren't in the Dictionary