using UnityEngine;
using System.Collections;

public class TankAI : MonoBehaviour {

	static LayerMask visibleLayers = 1 << LayerMask.NameToLayer ("Default");
	static Collider2D[] closeColliders = new Collider2D[50];
	Tank tank;
	Tank target;

	void Awake () {
		tank = GetComponent<Tank> ();
	}

	void Start () {
		StartCoroutine (SeekTarget());
	}

	void Update () {
		if (target) {
			tank.LookAt (target.transform.position);
			tank.Fire ();
			tank.Move (0);
		} else {
//			tank.RotateTowards (new Vector3 (0, 0, 0));
			tank.Move (1);
		}
	}
	
	IEnumerator SeekTarget () {
		while (!tank.dead) {
			yield return new WaitForSeconds (Random.Range (0.3f, 0.5f));

			bool currentTargetVisible = false;
			int count = Physics2D.OverlapCircleNonAlloc (transform.position, tank.fovDistance, closeColliders, visibleLayers);
			for (int i = 0; i < count; ++i) {
				Collider2D collider = closeColliders [i];
				Tank otherTank = collider.attachedRigidbody.GetComponent<Tank> ();
				if (!otherTank) {
					continue;
				}

				if (otherTank.team == tank.team) {
					continue;
				}

				//check fov visibility
				float toTank = Quaternion.FromToRotation (tank.tower.up, otherTank.transform.position - tank.transform.position).eulerAngles.z;
				if (Mathf.Abs (toTank) > tank.fovAngle / 2) {
					continue;
				}
				
				if (otherTank == target) {
					currentTargetVisible = true;
				}
				
				if (target == null) {
					target = otherTank;
					currentTargetVisible = true;
				}
			}
			if (!currentTargetVisible) {
				target = null;
			}
		}
	}
}

