using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tank : MonoBehaviour {

    public delegate void DeathAction (Tank tank);
    public static event DeathAction OnDeath;
    public static HashSet<Tank> allTanks = new HashSet<Tank> ();

    public int team = 0;
    public GameObject uiPrefab;
    public GameObject hitEffectPrefab;
    public GameObject deathEffectPrefab;
    public LineRenderer laserPrefab;

    [Header("Battle")]
    public float maxHealth = 100;
    public float health = 100;
    public bool immortal = false;
    public float damage = 100;
    public float attackCooldown = 1;

    [Header("Locomotion")]
    public float fovDistance = 5;
    public float fovAngle = 120;
    public float maxSpeed = 1;
    [Tooltip("Body turn rate (degrees per second)")]
    public float turnRate = 120;
    [Tooltip("Turret turn rate (degrees per second)")]
    public float turretTurnRate = 180;

    [HideInInspector]
    public new Transform transform;
    [HideInInspector]
    public Transform turret;
    [HideInInspector]
    public float attackCooldownRemains = 0f;

    new Renderer renderer;
    Rigidbody2D rigidbody;
    GameObject ui;

    void Awake () {
        transform = GetComponent<Transform> ();
        rigidbody = GetComponent<Rigidbody2D> ();
        renderer = GetComponent<Renderer> ();
        turret = transform.Find ("Turret");
        allTanks.Add (this);
    }

    void OnDestroy () {
        allTanks.Remove (this);
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
		float turretTurnRateRadians = turretTurnRate * Mathf.Deg2Rad;
		Vector3 direction = position - transform.position;
		direction.z = 0;
		direction = Vector3.RotateTowards (turret.up, direction, turretTurnRateRadians * Time.deltaTime, 0);
		direction.z = 0;
		turret.up = direction;
    }

    public void RotateTowards (Vector3 position) {
		float turnRateRadians = turnRate * Mathf.Deg2Rad;
		Vector3 direction = position - transform.position;
		direction.z = 0;
		direction = Vector3.RotateTowards (transform.up, direction, turnRateRadians * Time.deltaTime, 0);
		direction.z = 0;
		transform.up = direction;
    }

    public void Rotate (float direction) {
        transform.Rotate (0, 0, direction * turnRate * Time.deltaTime);
    }

    public void Move (float speed) {
        if (speed != 0) {
            rigidbody.MovePosition (transform.position + transform.up * speed * maxSpeed * Time.deltaTime);
        }
    }

    public void Fire () {
        if (attackCooldownRemains > 0) {
            return;
        }
        attackCooldownRemains = attackCooldown;

        Vector3 endPoint = turret.position + turret.up * fovDistance;
        RaycastHit2D hit = Physics2D.Linecast (turret.position, endPoint);

        bool laserVisible = renderer.isVisible;

        if (hit) {
            endPoint = hit.point;
            Tank tank = hit.rigidbody.GetComponent<Tank> ();
            if (tank) {
                if (tank.renderer.isVisible) {
                    laserVisible = true;
                    Instantiate (hitEffectPrefab, (Vector3)hit.point + new Vector3 (0, 0, -1), Quaternion.identity);
                }
                tank.TakeDamage (damage);
            }
        }

        if (laserVisible) {
            CreateLaser (transform.position, endPoint);
        }
    }

    void CreateLaser (Vector3 startPoint, Vector3 endPoint) {
        LineRenderer laser = Instantiate (laserPrefab, startPoint, Quaternion.identity) as LineRenderer;
        laser.SetPosition (0, startPoint);
        laser.SetPosition (1, endPoint);
        Destroy (laser, 0.1f);
        Destroy (laser.gameObject, 0.5f);
    }

    void TakeDamage (float damage) {
        if (immortal) {
            return;
        }

        health -= damage;
        if (health <= 0) {
            if (renderer.isVisible) {
                Instantiate (deathEffectPrefab, transform.position + new Vector3 (0, 0, -1), Quaternion.identity);
            }

            OnDeath (this);
            Destroy (gameObject);
        }
    }

    void OnDrawGizmosSelected () {
        if (!turret) {
            return;
        }

        Gizmos.DrawLine (turret.position, turret.position + turret.up * fovDistance);
        Gizmos.DrawLine (turret.position, turret.position + Quaternion.Euler (0, 0, fovAngle / 2) * turret.up * fovDistance);
        Gizmos.DrawLine (turret.position, turret.position + Quaternion.Euler (0, 0, -fovAngle / 2) * turret.up * fovDistance);
    }
}
