using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tank : MonoBehaviour {

	public int team = 0;

	public GameObject uiPrefab;
	public GameObject hitEffectPrefab;
	public GameObject deathEffectPrefab;
	public Material laserMaterial;

	[Header("Battle")]
	public float maxHealth = 100;
	public float health = 100;
	public bool dead = false;
	public bool immortal = false;
	public float damage = 100;
	public float attackCooldown = 1;

	[Header("Locomotion")]
	public float fovDistance = 5;
	public float fovAngle = 120;
	public float maxSpeed = 1;
	public float turnRate = 120; // degrees per second
	public float towerTurnRate = 180; // degrees per second

	[HideInInspector]
	public new Transform transform;

	[HideInInspector]
	public Transform tower;

	[HideInInspector]
	public float attackCooldownRemains = 0f;

	public delegate void DeathAction(Tank tank);
	public static event DeathAction OnDeath;

	new Renderer renderer;
	new Rigidbody2D rigidbody;
	GameObject ui;

	void Awake() {
		transform = GetComponent<Transform>();
		rigidbody = GetComponent<Rigidbody2D>();
		renderer = GetComponent<Renderer>();
		tower = transform.Find ("Tower");
	}

	void Update () {
		if (attackCooldownRemains > 0) {
			attackCooldownRemains = Mathf.Max (0, attackCooldownRemains - Time.deltaTime);
		}
	}

	void OnBecameVisible () {
		ui = Instantiate (uiPrefab, transform.position, Quaternion.identity) as GameObject;
		ui.transform.SetParent (transform);
	}

	void OnBecameInvisible () {
		if (ui) {
			Destroy (ui);
		}
	}

	public void LookAt (Vector3 position) {
		Quaternion targetRotation = Quaternion.FromToRotation (Vector3.up, position-transform.position);
		tower.rotation = Quaternion.RotateTowards (tower.rotation, targetRotation, towerTurnRate * Time.deltaTime);
	}

	public void RotateTowards (Vector3 position) {
		Quaternion targetRotation = Quaternion.FromToRotation (Vector3.up, position-transform.position);
		transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, turnRate * Time.deltaTime);
	}

	public void Rotate (float direction) {
		transform.Rotate (0, 0, -direction * turnRate * Time.deltaTime);
	}

	public void Move (float move) {
		if (move != 0) {
			rigidbody.MovePosition (transform.position + transform.up * move * maxSpeed * Time.deltaTime);
		}
	}

	public void Fire () {
		if (attackCooldownRemains > 0) {
			return;
		}
		attackCooldownRemains = attackCooldown;

		Vector3 endPoint = tower.position + tower.up * fovDistance;
		RaycastHit2D hit = Physics2D.Linecast (tower.position, endPoint);

		bool laserVisible = renderer.isVisible;

		if(hit) {
			endPoint = hit.point;
			Tank tank = hit.rigidbody.GetComponent<Tank>();
			if(tank) {
				if(tank.renderer.isVisible) {
					laserVisible = true;
					Instantiate (hitEffectPrefab, (Vector3)hit.point + new Vector3(0,0,-1), Quaternion.identity);
				}
				tank.TakeDamage(damage);
			}
		}

		if (laserVisible) {
			CreateLaser (tower.position, endPoint);
		}
	}

	void CreateLaser (Vector3 startPoint, Vector3 endPoint)
	{
		GameObject laser = new GameObject ("Laser");
		LineRenderer lineRenderer = laser.AddComponent<LineRenderer>();
		lineRenderer.SetVertexCount (2);
		lineRenderer.SetColors (Color.grey, Color.red);
		lineRenderer.SetWidth (0.02f, 0.025f);
		lineRenderer.SetPosition (0, startPoint);
		lineRenderer.SetPosition (1, endPoint);
		lineRenderer.material = laserMaterial;
		Destroy (laser, 0.1f);
	}

	void TakeDamage (float damage) {
		if (immortal) {
			return;
		}

		health -= damage;
		if (health <= 0) {
			if(renderer.isVisible) {
				Instantiate (deathEffectPrefab, transform.position + new Vector3(0,0,-1), Quaternion.identity);
			}

			OnDeath(this);

			dead = true;
			if(ui) {
				Destroy (ui.gameObject);
			}
			Destroy (gameObject);
		}
	}

	void OnDrawGizmosSelected () {
		if (!tower) {
			return;
		}

		Gizmos.DrawLine (tower.position, tower.position + tower.up * fovDistance);
		Gizmos.DrawLine (tower.position, tower.position + Quaternion.Euler (0, 0, fovAngle / 2) * tower.up * fovDistance);
		Gizmos.DrawLine (tower.position, tower.position + Quaternion.Euler (0, 0, -fovAngle / 2) * tower.up * fovDistance);
	}
}
