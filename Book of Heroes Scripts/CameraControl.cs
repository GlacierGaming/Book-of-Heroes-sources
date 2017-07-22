using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CameraControl : MonoBehaviour {
	// Control game camera and game scene
	
	public GameObject followTarget, storyBox, cinematicObject;
	public GameObject[] passedObj;
	public float moveSpeed;
	public AudioClip trainingMusic, combatMusic, bossMusic;
	public AudioSource audioSource;
	public MovieTexture[] cinematic;
	public bool cinematicPlaying;

	private Vector3 targetPos;
	private StoryScript storyScript;
	private int sceneNo;

	void Start () {
		Application.targetFrameRate = 70;

		// If it's a loaded game set the scene as it was then
		if(PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString()) > 0) {
			Vector3 setTargetPos = new Vector3(PlayerPrefs.GetFloat("PLAYERX" + PlayerPrefs.GetInt("SLOTNO").ToString()), PlayerPrefs.GetFloat("PLAYERY" + PlayerPrefs.GetInt("SLOTNO").ToString()), 0);
			followTarget.transform.position = setTargetPos;

			followTarget.GetComponent<PlayerControl>().health = PlayerPrefs.GetInt("PLAYERHP" + PlayerPrefs.GetInt("SLOTNO").ToString());

			for(int obj = 0; obj < PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString()); ++obj)
				passedObj[obj].SetActive(false);

			storyBox.GetComponent<StoryScript>().currentLine = PlayerPrefs.GetInt("CURRLINE" + PlayerPrefs.GetInt("SLOTNO").ToString());
			storyBox.GetComponent<StoryScript>().endLine = PlayerPrefs.GetInt("ENDLINE" + PlayerPrefs.GetInt("SLOTNO").ToString());

			GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().ResumeGame();

			storyBox.SetActive(true);
			storyBox.GetComponent<StoryScript>().DisableDialogBox();

			audioSource.clip = combatMusic;
			if(audioSource.isPlaying == false) audioSource.Play();
		}
		transform.position = new Vector3(followTarget.transform.position.x, followTarget.transform.position.y - 1.5f, transform.position.z);

		if (transform.name == "Main Camera") {
			storyScript = storyBox.GetComponent<StoryScript>();
			// If it's not a loaded game start the intro cinematic
			if(PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString()) == 0) {
				cinematicObject.GetComponent<GUITexture>().pixelInset = new Rect(0, 0, Screen.width,  Screen.height * 9f * Screen.width / 16f / Screen.height);
				cinematic[sceneNo].Play();
				cinematicPlaying = true;
				GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().cinematicPlaying = true;
			}
		}
	}

	void Update () {
		gameObject.GetComponent<Camera>().rect = new Rect(0, (1f - 9f * Screen.width / 16f / Screen.height) / 2f, 1, 9f * Screen.width / 16f / Screen.height);

		targetPos = new Vector3 (followTarget.transform.position.x, followTarget.transform.position.y - 1.5f, transform.position.z);
		transform.position = Vector3.Lerp (transform.position, targetPos, moveSpeed * Time.deltaTime);

		// Cinematic control
		if (transform.name == "Main Camera" && cinematicPlaying == true && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return))) {
			cinematic[sceneNo].Stop();
			sceneNo++;
			if (sceneNo < 3) {
				cinematicObject.GetComponent<GUITexture>().texture = cinematic[sceneNo];
				cinematic[sceneNo].Play();
			} else {
				cinematicPlaying = false;
				GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().cinematicPlaying = false;
				GameObject.Find("PauseMenu").GetComponent<PauseMenuScript>().ResumeGame();
				storyBox.SetActive(true);
			}
		}

		// Music control
		if (transform.name == "Main Camera") {
			if(cinematicPlaying == false && storyScript.currentLine == 0) {
				Destroy (cinematicObject);
				audioSource.clip = trainingMusic;
				if(audioSource.isPlaying == false) audioSource.Play();
			}
			if (storyScript.currentLine == 35) {
				audioSource.clip = combatMusic;
				if(audioSource.isPlaying == false) audioSource.Play();
			}
			if (GameObject.Find("FinalTrigger") == null) {
				audioSource.clip = bossMusic;
				if(audioSource.isPlaying == false) audioSource.Play();
			}
		}

		// Restart level
		if (followTarget.activeSelf == false) {
			if(PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO")) > 0)
            	SceneManager.LoadScene("test1");
			else 
				SceneManager.LoadScene("Meniu");
        }
	}

	public void LoadGame (int slotNo) {
		PlayerPrefs.SetInt("SLOTNO", slotNo);
		SceneManager.LoadScene("test1");
	}
}
