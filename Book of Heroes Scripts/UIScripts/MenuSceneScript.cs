using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSceneScript : MonoBehaviour {
	// Functions for main menu scene

	public int QualityNo;

	void Update () {
		gameObject.GetComponent<Camera>().rect = new Rect(0, (1f - 9f * Screen.width / 16f / Screen.height) / 2f, 1, 9f * Screen.width / 16f / Screen.height);
	}

	public void QuitGame () {
		Application.Quit();
	}

	public void DeleteSlot(int slotNo) {
		PlayerPrefs.SetInt("PASSEDOBJS" + slotNo.ToString(), 0);
		PlayerPrefs.SetString("SLOTNAME" + slotNo.ToString(), "Slot" + slotNo.ToString());
	}

	public void QualitySetting (int QualityNo) {
		QualitySettings.SetQualityLevel(QualityNo, true);
	}
}
