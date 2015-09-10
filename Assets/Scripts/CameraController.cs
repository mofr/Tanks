using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float minSize = 1;
    public float maxSize = 10;

    new Camera camera;

    void Awake () {
        camera = GetComponent<Camera> ();
    }

    void LateUpdate () {
        float scroll = Input.GetAxis ("Mouse ScrollWheel");
        camera.orthographicSize -= scroll;
        camera.orthographicSize = Mathf.Clamp (camera.orthographicSize, minSize, maxSize);

        if (target) {
            transform.position = new Vector3 (target.position.x, target.position.y, transform.position.z);
        }
    }
}