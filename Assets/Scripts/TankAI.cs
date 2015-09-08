using UnityEngine;
using System.Collections;

public class TankAI : MonoBehaviour {

	static LayerMask seenLayers = 1 << LayerMask.NameToLayer ("Default");

	Tank tank;

	Collider2D[] closeColliders = new Collider2D[30];
	Tank currentTarget;

	void Awake() {
		tank = GetComponent<Tank>();
	}

	void Start () {
		StartCoroutine (SeekTanks ());
	}

	void Update () {
		if (currentTarget) {
			tank.LookAt (currentTarget.transform.position);
			tank.Fire ();
		} else {
			tank.Move(1);
		}
	}
	
	IEnumerator SeekTanks() {
		while (!tank.dead) {
			bool currentTargetVisible = false;
			int count = Physics2D.OverlapCircleNonAlloc(transform.position, tank.fovDistance, closeColliders, seenLayers);
			for(int i = 0; i < count; ++i) {
				Collider2D collider = closeColliders[i];
				Tank otherTank = collider.attachedRigidbody.GetComponent<Tank>();
				if(!otherTank) continue;
				
				//todo check tank visibility
				
				if(otherTank == currentTarget) {
					currentTargetVisible = true;
				}
				
				if(otherTank.team != tank.team && currentTarget == null) {
					currentTarget = otherTank;
					currentTargetVisible = true;
				}
			}
			if(!currentTargetVisible) {
				currentTarget = null;
			}
			yield return new WaitForSeconds(Random.Range (0.3f, 0.5f));
		}
	}
}

