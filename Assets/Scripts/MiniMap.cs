using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MiniMap : MonoBehaviour {

    public RawImage mapImage;
    public int width = 128;
    public int height = 128;
    public Color background = new Color (0, 0, 0, 0);

    Texture2D texture;
    Color[] emptyPixels;
    
    void Start () {
        texture = new Texture2D (height, width, TextureFormat.ARGB32, false, true);
        emptyPixels = texture.GetPixels ();
        for (int i = 0; i < emptyPixels.Length; ++i) {
            emptyPixels [i] = background;
        }
        texture.Apply ();

        StartCoroutine (UpdateMap ());
    }

    IEnumerator UpdateMap () {
        while (this) {
            texture.SetPixels (emptyPixels);

            Tank[] tanks = GameObject.FindObjectsOfType<Tank> ();
            for (int i = 0; i < tanks.Length; ++i) {
                Tank tank = tanks [i];
                Color color;
                if (tank.tag == "Player") {
                    color = Color.white;
                } else {
                    color = tank.team == 0 ? Color.green : Color.red;
                }
                int x = (int)((tank.transform.position.x + Level.instance.worldSize / 2) * texture.width / Level.instance.worldSize);
                int y = (int)((tank.transform.position.y + Level.instance.worldSize / 2) * texture.height / Level.instance.worldSize);
                texture.SetPixel (x, y, color);
            }

            texture.Apply ();

            mapImage.texture = texture;

            yield return new WaitForSeconds (1);
        }
    }
}
