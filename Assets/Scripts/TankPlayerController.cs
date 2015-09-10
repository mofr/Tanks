using UnityEngine;
using System.Collections;

public class TankPlayerController : MonoBehaviour {

    Tank tank;

    void Awake () {
        tank = GetComponent<Tank> ();
    }

    void Update () {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
        tank.LookAt (mousePos);

        tank.Move (Input.GetAxisRaw ("Vertical"));
        tank.Rotate (-Input.GetAxisRaw ("Horizontal"));

        if (Input.GetButton ("Fire1")) {
            tank.Fire ();
        }

        if (Input.GetButtonDown ("Immortal")) {
            tank.immortal = !tank.immortal;
        }
    }
}
