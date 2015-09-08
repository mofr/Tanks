using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	public float fieldSize;
	public int tankCount;
	public Tank tankPrefab;
	public Material terrainMaterial;

	void Awake () {
		GenerateTerrain ();
		GenerateTanks ();
	}

	void GenerateTerrain() {
		GameObject terrain = new GameObject ("Terrain");
		terrain.isStatic = true;
		terrain.transform.position = new Vector3 (0, 0, 1);
		MeshRenderer meshRenderer = terrain.AddComponent<MeshRenderer> ();
		meshRenderer.material = terrainMaterial;
		MeshFilter meshFilter = terrain.AddComponent<MeshFilter> ();
		Mesh mesh = new Mesh ();
		mesh.vertices = new Vector3[]{new Vector3(-fieldSize, fieldSize), new Vector3(-fieldSize, -fieldSize), new Vector3(fieldSize, -fieldSize), new Vector3(fieldSize, fieldSize)};
		mesh.uv = new Vector2[]{new Vector2(0, 0), new Vector2(0, 1)*fieldSize/10, new Vector2(1, 1)*fieldSize/10, new Vector2(1, 0)*fieldSize/10};
		mesh.triangles = new int[]{2, 1, 0, 3, 2, 0};
		meshFilter.mesh = mesh;
	}

	void GenerateTanks() {
		Tank playerTank = Instantiate(tankPrefab, new Vector3(0,0), Quaternion.identity) as Tank;
		playerTank.name = "Player Tank";
		playerTank.tag = "Player";
		playerTank.immortal = true;

		for (int i = 0; i < tankCount; ++i) {
			Vector2 position = new Vector2(Random.Range(-fieldSize, fieldSize),
			                               Random.Range(-fieldSize, fieldSize));
			Tank tank = Instantiate(tankPrefab, position, Quaternion.identity) as Tank;
			tank.name = "Tank #" + i;
			tank.gameObject.AddComponent<TankAI>();
			tank.team = i%2;
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (new Vector3(0,0), new Vector3(fieldSize * 2, fieldSize * 2));
	}
}
