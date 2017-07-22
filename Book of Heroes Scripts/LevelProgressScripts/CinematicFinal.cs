using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CinematicFinal : MonoBehaviour {
	// Control of the cinematic at the end of the road

	[System.Serializable]
	public class Pair {

		public MovieTexture scene;
		public bool loop;
	} 

	public GameObject cinematicObject;
	public Pair[] cinematic;

	private int sceneNo;
	private bool cinematicPlaying;

	void Start () {
		cinematicObject.GetComponent<GUITexture>().pixelInset = new Rect(0, 0, Screen.width, Screen.height * 9f * Screen.width / 16f / Screen.height);
	}
	
	void Update () {
		if(cinematicPlaying && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))) {
			cinematic[sceneNo].scene.Stop();
			sceneNo++;
			if (sceneNo < cinematic.Length) {
				cinematicObject.GetComponent<GUITexture>().texture = cinematic[sceneNo].scene;
				cinematic[sceneNo].scene.Play();
				if (cinematic[sceneNo].loop)
					cinematic[sceneNo].scene.loop = true;
			} else {
				GameObject.Find("Mage").transform.position = new Vector3 (-25, 89, 0);
				GameObject.Find("Main Camera").transform.position = new Vector3 (-25, 89, -7.6f);
				GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().ResumeGame();
				GameObject.Find("Mage").GetComponent<PlayerControl>().enabled = true;
				Destroy(cinematicObject);
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		if (!GameObject.Find("AI_Goblin(Clone)") && !GameObject.Find("AI_Orc(Clone)") && !GameObject.Find("AI_Troll(Clone)")) {
			cinematicObject.SetActive(true);
			cinematic[sceneNo].scene.Play();
			if (cinematic[sceneNo].loop)
				cinematic[sceneNo].scene.loop = true;
			cinematicPlaying = true;
			GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().cinematicPlaying = true;
			GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().DisableUI();
			GameObject.Find("Mage").GetComponent<Rigidbody>().velocity = Vector3.zero;
			GameObject.Find("Mage").GetComponent<PlayerControl>().enabled = false;
		}
	}
}
