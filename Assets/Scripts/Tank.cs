using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tank : MonoBehaviour {

	public int team = 0;

	public GameObject hitEffectPrefab;
	public GameObject deathEffectPrefab;

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
	public Transform body;

	Renderer bodyRenderer;
	Rigidbody2D rigidBody;
	Collider2D bodyCollider;
	Transform ui;
	Image healthBar;
	Image attackRecoveryBar;

	float speed = 0f;
	float attackCooldownRemains = 0f;

	static Material laserMaterial = new Material(Shader.Find("Particles/Additive"));

	void Awake() {
		transform = GetComponent<Transform>();
		rigidBody = GetComponentInChildren<Rigidbody2D>();
		tower = transform.Find ("Tower");
		body = transform.Find ("Body");
		bodyCollider = body.GetComponent<Collider2D> ();
		ui = transform.Find ("UI");
		healthBar = transform.Find ("UI/HealthBar").GetComponent<Image>();
		attackRecoveryBar = transform.Find ("UI/AttackRecoveryBar").GetComponent<Image>();

		bodyRenderer = body.GetComponent<Renderer>();
		ui.SetParent (transform.parent);
	}

	void Update () {
		if (attackCooldownRemains > 0) {
			attackCooldownRemains = Mathf.Max (0, attackCooldownRemains - Time.deltaTime);
		}
	}

	void LateUpdate() {
		ui.gameObject.SetActive (bodyRenderer.isVisible);

		if (ui.gameObject.activeInHierarchy) {
			ui.position = transform.position;

			healthBar.color = team == 0 ? Color.green : Color.red;
			healthBar.transform.localScale = new Vector3 (Mathf.Clamp01 (health / maxHealth), 1);

			attackRecoveryBar.transform.localScale = new Vector3 (Mathf.Clamp01 (attackCooldownRemains / attackCooldown), 1);
		}
	}

	void FixedUpdate() {
		if (speed != 0) {
			rigidBody.MovePosition (transform.position + body.up * speed * Time.deltaTime);
		}
	}

	public void LookAt(Vector3 position) {
		Quaternion targetRotation = Quaternion.FromToRotation (Vector3.up, position-transform.position);
		tower.rotation = Quaternion.RotateTowards (tower.rotation, targetRotation, towerTurnRate * Time.deltaTime);
	}

	public void Rotate(float direction) {
		body.Rotate (0, 0, -direction * turnRate * Time.deltaTime);
	}

	public void Move (float move) {
		speed = move * maxSpeed;
	}

	public void Fire () {
		if (attackCooldownRemains > 0) {
			return;
		}
		attackCooldownRemains = attackCooldown;

		Vector3 endPoint = tower.position + tower.up * fovDistance;
		RaycastHit2D hit = Physics2D.Linecast (tower.position, endPoint);

		bool laserVisible = bodyRenderer.isVisible;

		if(hit) {
			endPoint = hit.point;
			Tank tank = hit.rigidbody.GetComponent<Tank>();
			if(tank) {
				if(tank.bodyRenderer.isVisible) {
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
			if(bodyRenderer.isVisible) {
				Instantiate (deathEffectPrefab, transform.position + new Vector3(0,0,-1), Quaternion.identity);
			}

			dead = true;
			gameObject.SetActive (false);
			ui.gameObject.SetActive(false);
		}
	}

	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere (GetComponent<Transform>().position, fovDistance);
	}
}
