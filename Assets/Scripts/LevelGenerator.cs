using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour
{
	
	public Tank tankPrefab;
	public Material terrainMaterial;
	public static LevelGenerator instance;
	static int pixelsPerUnit = 100;

	void Awake ()
	{
		instance = this;
	}

	public void Generate (int tankCount, int worldSize)
	{
		GenerateTerrain (worldSize);
		GenerateTanks (tankCount, worldSize);
		GeneratePlayerTank ();
	}

	void GenerateTerrain (int worldSize)
	{
		GameObject terrain = new GameObject ("Terrain");
		terrain.transform.position = new Vector3 (0, 0, 1);
		MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer> ();
		meshRenderer.material = terrainMaterial;
		MeshFilter meshFilter = terrain.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		mesh.vertices = new Vector3[] {
			new Vector3 (-worldSize, worldSize)/2,
			new Vector3 (-worldSize, -worldSize)/2,
			new Vector3 (worldSize, -worldSize)/2,
			new Vector3 (worldSize, worldSize)/2
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

	void GenerateTanks (int tankCount, int worldSize)
	{
		for (int i = 0; i < tankCount; ++i) {
			Vector2 position = new Vector2 (Random.Range (-worldSize, worldSize)/2,
			                                Random.Range (-worldSize, worldSize)/2);
			Tank tank = Instantiate (tankPrefab, position, Quaternion.identity) as Tank;
			tank.name = "Tank #" + i;
			tank.gameObject.AddComponent<TankAI> ();
			tank.team = i % 2;
		}
	}

	void GeneratePlayerTank ()
	{
		Tank playerTank = Instantiate (tankPrefab, new Vector3 (0, 0), Quaternion.identity) as Tank;
		playerTank.gameObject.AddComponent<PlayerInput> ();
		playerTank.name = "Player Tank";
		playerTank.tag = "Player";
		playerTank.immortal = true;
		Camera.main.GetComponent<CameraController> ().target = playerTank.transform;
	}
}
