using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ProtoTurtle.BitmapDrawing;

public class MiniMap : MonoBehaviour {

    public RawImage mapImage;
    public int width = 128;
    public int height = 128;
    public Color backgroundColor = new Color (0, 0, 0, 0);
    public Color cameraColor = new Color (0.5f, 0.5f, 0.5f, 0.5f);

    Texture2D texture;
    
    void Start () {
        texture = new Texture2D (height, width, TextureFormat.ARGB32, false, true);
        StartCoroutine (UpdateMap ());
    }

    IEnumerator UpdateMap () {
        Color[] backgroundPixels = texture.GetPixels ();
        for (int i = 0; i < backgroundPixels.Length; ++i) {
            backgroundPixels [i] = backgroundColor;
        }

        while (this) {
            texture.SetPixels (backgroundPixels);

            Vector3 translation = new Vector3 (texture.width / 2, 
                                               texture.height / 2);
            Vector3 scale = new Vector3 (texture.width / Level.instance.worldSize, 
                                         -texture.height / Level.instance.worldSize);
            Matrix4x4 trs = Matrix4x4.TRS (translation, Quaternion.identity, scale);

            foreach (Tank tank in Tank.allTanks) {
                Color color = tank.team == 0 ? Color.green : Color.red;
                float minRadius = 1;
                if (tank.tag == "Player") {
                    color = Color.white;
                    minRadius = 2;
                }
                float radius = Mathf.Max (minRadius, scale.x);
                Vector3 point = trs.MultiplyPoint3x4 (tank.transform.position);
                texture.DrawFilledCircle ((int)point.x, (int)point.y, (int)radius, color);
            }

            texture.Apply ();

            mapImage.texture = texture;

            yield return new WaitForSeconds (0.15f);
        }
    }
}
