using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class LoadButtonScript : MonoBehaviour {
	// Loading button control

	public int slotNo;
	public GameObject newGameObject;

	private bool gameSaved;

	void Start () {
		if(SceneManager.GetActiveScene().name != "Meniu" && PlayerPrefs.GetInt("PASSEDOBJS" + slotNo.ToString()) == 0)
			gameObject.GetComponent<Button>().interactable = false;
	}

	void Update () {
		if(!gameSaved && PlayerPrefs.GetInt("PASSEDOBJS" + slotNo.ToString()) > 0) {
			gameObject.GetComponent<Button>().interactable = true;
			gameSaved = true;
		}

		if(PlayerPrefs.GetInt("PASSEDOBJS" + slotNo.ToString()) > 0)
			transform.GetChild(0).GetComponent<Text>().text = PlayerPrefs.GetString("SLOTNAME" + slotNo.ToString());
		else 
			transform.GetChild(0).GetComponent<Text>().text = "Slot" + slotNo.ToString();
	}

	public void SlotButtonPressed(int slotNo) {
		if(PlayerPrefs.GetInt("PASSEDOBJS" + slotNo.ToString()) > 0) 
			StartGame(slotNo);
		else 
			newGameObject.SetActive(true);
	}

	public void StartGame (int slotNo) {
		PlayerPrefs.SetInt("SLOTNO", slotNo);
		SceneManager.LoadScene("test1");
	}

	public void SaveName(int slotNo) {
		if(newGameObject.transform.GetChild(3).GetComponent<InputField>().text == "")
			return;
		PlayerPrefs.SetString("SLOTNAME" + slotNo.ToString(), newGameObject.transform.GetChild(3).GetComponent<InputField>().text);
	}
}
