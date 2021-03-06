using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

    public static Level instance;
    public Tank tankPrefab1;
    public Tank tankPrefab2;
    public Material terrainMaterial;
    public float pixelsPerUnit = 100;
    public float worldSize;

    void Awake () {
        instance = this;
    }

    public void Generate (int tankCount, int worldSize) {
        this.worldSize = worldSize;

        GenerateTerrain (worldSize);
        GeneratePlayerTank ();
        GenerateTanks (tankCount, worldSize);
    }

    void GenerateTerrain (int worldSize) {
        GameObject terrain = new GameObject ("Terrain");
        terrain.transform.position = new Vector3 (0, 0, 1);
        MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer> ();
        meshRenderer.material = terrainMaterial;
        MeshFilter meshFilter = terrain.AddComponent<MeshFilter> ();
        Mesh mesh = new Mesh ();
        mesh.vertices = new Vector3[] {
                        new Vector3 (-worldSize, worldSize) / 2,
                        new Vector3 (-worldSize, -worldSize) / 2,
                        new Vector3 (worldSize, -worldSize) / 2,
                        new Vector3 (worldSize, worldSize) / 2
        };
        float uvScale = worldSize * pixelsPerUnit / terrainMaterial.mainTexture.width;
        mesh.uv = new Vector2[] {
                  new Vector2 (0, 0),
                  new Vector2 (0, 1) * uvScale,
                  new Vector2 (1, 1) * uvScale,
                  new Vector2 (1, 0) * uvScale
        };
        mesh.triangles = new int[]{2, 1, 0, 3, 2, 0};
        meshFilter.mesh = mesh;
    }

    void GenerateTanks (int tankCount, int worldSize) {
        float between = Mathf.Min (worldSize / 10, 10);
        for (int i = 0; i < tankCount; ++i) {
            int team = i % 2;
            Quaternion rotation;
            Vector2 position;
            if (team == 0) {
                rotation = Quaternion.Euler (0, 0, 0);
                position = new Vector2 (Random.Range (-worldSize, worldSize) / 2,
                                        Random.Range (-worldSize, -between) / 2);
            } else {
                rotation = Quaternion.Euler (0, 0, 180);
                position = new Vector2 (Random.Range (-worldSize, worldSize) / 2,
                                        Random.Range (between, worldSize) / 2);
            }
            Tank prefab = team == 0 ? tankPrefab1 : tankPrefab2;
            Tank tank = Instantiate (prefab, position, rotation) as Tank;
            tank.name = "Tank #" + i;
            tank.gameObject.AddComponent<TankBot> ();
            tank.team = team;
        }
    }

    void GeneratePlayerTank () {
        Tank playerTank = Instantiate (tankPrefab1, new Vector3 (0, 0), Quaternion.identity) as Tank;
        playerTank.gameObject.AddComponent<TankPlayerController> ();
        playerTank.name = "Player Tank";
        playerTank.tag = "Player";
        playerTank.immortal = true;
        playerTank.maxSpeed *= 1.5f;
        Camera.main.GetComponent<CameraController> ().target = playerTank.transform;
    }
}
