using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public Transform target;

	void LateUpdate () {
		if (target) {
			transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
		}
	}
}