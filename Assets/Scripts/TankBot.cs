using UnityEngine;
using System.Collections;

public class TankBot : MonoBehaviour {

    static LayerMask visibleLayers = 1 << LayerMask.NameToLayer ("Default");
    static Collider2D[] closeColliders = new Collider2D[50];
    Tank tank;
    Tank target;
    Vector3 destination;
    bool moving = false;

    void Awake () {
        tank = GetComponent<Tank> ();
    }

    void Start () {
        StartCoroutine (SeekTarget ());
        StartCoroutine (UpdateDestination ());
    }

    void Update () {
        if (target) {
            tank.RotateTowards (target.transform.position);
            tank.LookAt (target.transform.position);
            if (Vector3.Angle (tank.turret.up, target.transform.position - tank.transform.position) < 1) {
                tank.Fire ();
            }

            //keep target distance
            float targetDistance = (target.transform.position - tank.transform.position).magnitude;
            if (targetDistance > tank.fovDistance * 2 / 3) {
                tank.Move (1.0f);
            } 
            if (targetDistance < tank.fovDistance / 2) {
                tank.Move (-0.5f);
            }
        } else {
            if (moving) {
                tank.RotateTowards (destination);
                tank.LookAt (destination);
                tank.Move (0.7f);
            }
        }
    }

    IEnumerator UpdateDestination () {
        //move to frontier
        destination = new Vector3 (tank.transform.position.x, 0);

        while (tank) {
            yield return new WaitForSeconds (Random.Range (1.0f, 2.0f));

            if ((destination - tank.transform.position).magnitude > tank.fovDistance / 2) {
                moving = true;
            } else {
                moving = false;

                //patrol world center
                destination = Random.insideUnitCircle * tank.fovDistance * 4;
            }
        }
    }
    
    IEnumerator SeekTarget () {
        while (tank) {
            yield return new WaitForSeconds (Random.Range (0.15f, 0.3f));

            bool currentTargetVisible = false;
            int count = Physics2D.OverlapCircleNonAlloc (tank.transform.position, tank.fovDistance, closeColliders, visibleLayers);
            for (int i = 0; i < count; ++i) {
                Collider2D collider = closeColliders [i];
                Tank otherTank = collider.GetComponent<Tank> ();
                if (!otherTank) {
                    continue;
                }

                if (otherTank.team == tank.team) {
                    continue;
                }

                //check angular visibility
                float angleToTarget = Vector3.Angle (tank.turret.up, otherTank.transform.position - tank.transform.position);
                if (Mathf.Abs (angleToTarget) > tank.fovAngle / 2) {
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

