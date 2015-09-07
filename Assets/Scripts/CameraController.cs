using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

	public Transform target;
	public float interpVelocity = 5f;
	public Vector2 offset;
	
	Vector3 lastTargetPos;

	void Start () {
		if (!target) {
			target = GameObject.FindWithTag("Player").transform;
		}

		transform.position = CalcTargetPos ();
	}

	void LateUpdate () {
		Vector3 targetPos = CalcTargetPos ();
		transform.position = Vector3.Lerp (transform.position, targetPos, interpVelocity * Time.deltaTime);
	}

	Vector3 CalcTargetPos() {
		Vector3 targetPos = target.position;
		targetPos.z = transform.position.z;
		targetPos += (Vector3)offset;

		return targetPos;
	}
}