using UnityEngine;
using System.Collections;

public class LevelGenerator : MonoBehaviour {

	public int tankCount;
	public Bounds bounds;
	public Tank tankPrefab;

	void Awake () {
		Generate ();
	}

	void Generate() {
		Tank playerTank = Instantiate(tankPrefab, new Vector3(0,0), Quaternion.identity) as Tank;
		playerTank.name = "Player Tank";
		playerTank.tag = "Player";

		for (int i = 0; i < tankCount; ++i) {
			Vector2 position = new Vector2(Random.Range(bounds.min.x, bounds.max.x),
			                               Random.Range(bounds.min.y, bounds.max.y));
			Tank tank = Instantiate(tankPrefab, position, Quaternion.Euler(0,0,Random.Range (0, 360))) as Tank;
			tank.name = "Tank #" + i;
			tank.gameObject.AddComponent<TankAI>();
			tank.team = i%2;
			if(tank.team == 1) {
				foreach(SpriteRenderer spriteRenderer in tank.GetComponentsInChildren<SpriteRenderer>()) {
					spriteRenderer.color = Color.red;
				}
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (bounds.center, bounds.extents * 2);
	}
}
