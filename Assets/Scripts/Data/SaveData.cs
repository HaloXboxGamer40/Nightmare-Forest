using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData {

    public int level;
    public List<ChunkData> chunks = new List<ChunkData>();
    public float[] playerPos = new float[3];
    public float[] playerRot = new float[2];

    public SaveData(int level) {
        this.level = level;
    }

    public void AddChunk(ChunkData chunk) {

        chunks.Add(chunk);

    }

    public void UpdatePlayerPosition(Transform player, Transform camera) {

        // Calculate pitch and yaw
        float pitch = NormalizeAngle(camera.localEulerAngles.x);
        pitch = Mathf.Clamp(pitch, -89f, 89f);

        float yaw = NormalizeAngle(player.eulerAngles.y);

        this.playerPos[0] = player.position.x;
        this.playerPos[1] = player.position.y;
        this.playerPos[2] = player.position.z;

        this.playerRot[0] = pitch;
        this.playerRot[1] = yaw;

    }

    private float NormalizeAngle(float a) {
        // converts 360 degree rotation to 180 degree rotation
        if (a > 180f) a -= 360f;
        return a;
    }

}
