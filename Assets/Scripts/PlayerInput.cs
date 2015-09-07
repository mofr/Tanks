using UnityEngine;
using System.Collections;

public class PlayerInput : MonoBehaviour {

	Tank tank;

	void Start () {
		tank = GameObject.FindWithTag ("Player").GetComponent<Tank>();
	}

	void Update () {
		Vector3 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		pos.z = 0;
		tank.LookAt (pos);

		tank.Move(Input.GetAxisRaw ("Vertical"));

		if (Input.GetButtonDown ("Fire1")) {
			tank.Fire();
		}
	}
}
