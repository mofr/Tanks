using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tank : MonoBehaviour {

	public int team = 0;
	public float health = 100;
	public bool dead = false;
	public float damage = 100;
	public float fovDistance = 5;
	public float fovAngle = 120;
	public float maxSpeed = 1;

	[HideInInspector]
	public new Transform transform;

	[HideInInspector]
	public Transform tower;

	float speed = 0f;

	void Awake() {
		transform = GetComponent<Transform>();
		tower = transform.Find ("Tower");
	}

	void Update () {
		Vector3 dir = transform.TransformDirection (Vector3.up);
		transform.position += dir * speed * Time.deltaTime;
	}

	public void LookAt(Vector3 pos) {
		Quaternion rotation = Quaternion.FromToRotation (Vector3.up, pos-tower.position);
		tower.rotation = Quaternion.Euler (0,0,rotation.eulerAngles.z);
	}

	public void Move (float move)
	{
		speed = move * maxSpeed;
	}

	public void Fire ()
	{
		RaycastHit2D hit = Physics2D.Linecast (tower.position, tower.position + tower.up * fovDistance);
		if(hit) {
			Tank tank = hit.collider.GetComponent<Tank>();
			if(tank) {
				tank.TakeDamage(damage);
			}
		}
	}

	void TakeDamage (float damage)
	{
		health -= damage;
		if (health <= 0) {
			dead = true;
			gameObject.SetActive (false);
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere (transform.position, fovDistance);
	}
}
