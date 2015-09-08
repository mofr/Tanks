using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GeneratePanel : MonoBehaviour {

	public InputField tankCountInput;
	public InputField worldSizeInput;
	
	public void OnPlay() {
		int tankCount;
		if (!int.TryParse (tankCountInput.text, out tankCount)) {
			return;
		}

		int worldSize;
		if (!int.TryParse (worldSizeInput.text, out worldSize)) {
			return;
		}

		LevelGenerator.instance.Generate (tankCount, worldSize);

		gameObject.SetActive(false);
	}
}
