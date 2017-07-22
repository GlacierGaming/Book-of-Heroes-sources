using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGameScript : MonoBehaviour {

	public int passedObjs, currLine, endLine;
	public GameObject trigger;
	public Transform screenCanvas;

	void Start () {
		if(!trigger.activeSelf) {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(true);
		}
	}

	void OnTriggerEnter() {
		if(trigger && trigger.activeSelf || transform.GetChild(1).gameObject.activeSelf)
			return;

		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(true);

		// Saving
		PlayerPrefs.SetFloat("PLAYERX" + PlayerPrefs.GetInt("SLOTNO").ToString(), GameObject.Find("Mage").transform.position.x);
		PlayerPrefs.SetFloat("PLAYERY" + PlayerPrefs.GetInt("SLOTNO").ToString(), GameObject.Find("Mage").transform.position.y);
		PlayerPrefs.SetInt("PLAYERHP" + PlayerPrefs.GetInt("SLOTNO").ToString(), GameObject.Find("Mage").GetComponent<PlayerControl>().health);
		PlayerPrefs.SetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString(), passedObjs);
		PlayerPrefs.SetInt("CURRLINE" + PlayerPrefs.GetInt("SLOTNO").ToString(), currLine);
		PlayerPrefs.SetInt("ENDLINE" + PlayerPrefs.GetInt("SLOTNO").ToString(), endLine);

		Instantiate(screenCanvas.GetChild(0).gameObject, screenCanvas).SetActive(true);
	}
}
