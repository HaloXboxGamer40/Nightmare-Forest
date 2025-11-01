using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Chunk {

    // Mesh data stuff
    List<Vector3> verts = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();
    List<int> triangles = new List<int>();

    // Without these refrences we wouldn't be able to generate a mesh
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    MeshCollider meshCollider;
 
    // ChunkObject refrence, MUST be public
    public GameObject chunk;

    // Keep track of where we are in the world and our material
    Vector2Int chunkCoord;
    Material material;

    // Just to cache our chunksize here
    int ChunkSize = GameData.ChunkWidth;

    // Generation values I probably should store in GameData
    float scale = 0.1f;
    float heightMultiplier = 1f;

    float offset;
    bool _isActive;

    // We need to know if we're active or not
    public bool isActive {

        get { return _isActive; }
        set {
            _isActive = value;
            chunk.SetActive(value);
        }

    }

    public Chunk(Vector2Int position, Material material) {
        
        // Creates the game object and adds stuff to it that we need
        this.material = material;
        chunk = new GameObject();
        meshFilter = chunk.AddComponent<MeshFilter>();
        meshRenderer = chunk.AddComponent<MeshRenderer>();
        chunk.name = "Chunk: (" + position.x + ", " + position.y + ")";
        meshCollider = chunk.AddComponent<MeshCollider>();

        chunk.transform.SetParent(World.Instance.transform);

        // Sets up position data
        chunkCoord = position;
        offset = (chunkCoord.x * chunkCoord.y) * GameData.ChunkWidth;

        chunk.transform.position = new Vector3Int(position.x * ChunkSize, 0, position.y * ChunkSize);

        // Generate Chunk and create the mesh
        GenerateChunk();
        CreateMesh();

        meshCollider.sharedMesh = meshFilter.mesh;
    }
    
    private void GenerateChunk() {
        // Usless for now but just to keep us safe in the future
        verts.Clear();
        uvs.Clear();
        triangles.Clear();

        // Offset so our positional data doesnt go into the negatives, otherwise we get an ugly seam in the middle of our chunks
        float offsetX = (chunkCoord.x * ChunkSize) + GameData.WorldSize;
        float offsetY = (chunkCoord.y * ChunkSize) + GameData.WorldSize;

        int vertPerLine = ChunkSize + 1; // vertices per row

        // vertices & UVs generation
        for (int x = 0; x <= ChunkSize; x++) {
            for (int z = 0; z <= ChunkSize; z++) {
                float perlin = Mathf.PerlinNoise((x + offsetX) * scale, (z + offsetY) * scale);
                float y = perlin * heightMultiplier;
                verts.Add(new Vector3(x, y, z));
                uvs.Add(new Vector2((float)x / ChunkSize, (float)z / ChunkSize));
            }
        }

        // triangles generator
        for (int x = 0; x < ChunkSize; x++) {
            for (int z = 0; z < ChunkSize; z++) {
                
                // Think of the vertices on a square
                int bottomL = x * vertPerLine + z; // Bottom Left
                int bottomR = bottomL + 1; // Bottom Right
                int topL = bottomL + vertPerLine; // Top Left
                int topR = topL + 1; // Top Right

                // Triangle 1
                triangles.Add(bottomL);
                triangles.Add(topR);
                triangles.Add(topL);

                // Triangle 2
                triangles.Add(bottomL);
                triangles.Add(bottomR);
                triangles.Add(topR);
            }
        }
    }


    private void CreateMesh() {

        // Create mesh and add data 
        Mesh mesh = new Mesh();
        mesh.SetVertices(verts);
        mesh.SetTriangles(triangles, 0);
        mesh.SetUVs(0, uvs);

        // Recauculate normals and bounds to have correct triangle faces
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // add our material
        meshRenderer.sharedMaterial = material;

        // Render the mesh!
        meshFilter.mesh = mesh;
    }
}

/*~~~ NOTED BUGS: ~~~*/
// Every chunk appears to have a werid boarder around it made with the mesh