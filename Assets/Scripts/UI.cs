using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI : MonoBehaviour {

    public GameObject generatePanel;
    public InputField tankCountInput;
    public InputField worldSizeInput;
    public GameObject scorePanel;
    public Text team1CountText;
    public Text team2CountText;
    public GameObject mapPanel;
    public Text fpsText;

    int team1Count = 0;
    int team2Count = 0;
    float deltaTime = 0;

    void Awake () {
        Tank.OnDeath += OnTankDeath;
    }

    void OnDestroy () {
        Tank.OnDeath -= OnTankDeath;
    }

    void Update () {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = ((int)fps).ToString () + " fps";

        if (Input.GetButtonDown ("Cancel")) {
            Application.LoadLevel(Application.loadedLevel);
        }
    }
    
    public void OnPlay () {
        int tankCount;
        if (!int.TryParse (tankCountInput.text, out tankCount)) {
            return;
        }

        int worldSize;
        if (!int.TryParse (worldSizeInput.text, out worldSize)) {
            return;
        }

        Level.instance.Generate (tankCount, worldSize);

        generatePanel.SetActive (false);
        scorePanel.SetActive (true);
        mapPanel.SetActive (true);

        team1Count = tankCount / 2 + 1; // + 1 player tank
        team2Count = tankCount - tankCount / 2;

        team1CountText.text = team1Count.ToString ();
        team2CountText.text = team2Count.ToString ();
    }

    void OnTankDeath (Tank tank) {
        if (tank.team == 0) {
            --team1Count;
        } else {
            --team2Count;
        }
        team1CountText.text = team1Count.ToString ();
        team2CountText.text = team2Count.ToString ();
    }
}
