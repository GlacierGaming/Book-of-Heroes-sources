using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour {
	// Pause menu button functions

	public Canvas regularCanvas, pauseMenuCanvas;
    public GameObject storyBox;

	[HideInInspector]
	public bool textBox, cinematicPlaying;

    private int atLine;
    private bool isPaused;

    void Update() {
		if (!cinematicPlaying && Input.GetKeyDown(KeyCode.Escape)) {
			if(isPaused) {
				ResumeGame();
			} else {
				PauseGame();
			}
        }
    }
	
	public void PauseGame () {
		if (storyBox.GetComponent<StoryScript>().isActive || storyBox.GetComponent<StoryScript>().currentLine < 30) {
			atLine = storyBox.GetComponent<StoryScript>().currentLine;
			storyBox.GetComponent<StoryScript>().DisableDialogBox();
			textBox = true;
		}
		isPaused = true;
		Time.timeScale = 0f;
		regularCanvas.gameObject.SetActive(false);
		pauseMenuCanvas.gameObject.SetActive(true);
		GameObject.Find("Mage").GetComponent<PlayerControl>().enabled = false;
	}

	public void ResumeGame () {
		if (textBox) {
			storyBox.GetComponent<StoryScript>().currentLine = atLine;
			storyBox.GetComponent<StoryScript>().EnableDialogBox();
			storyBox.SetActive(true);
			textBox = false;
		}
		isPaused = false;
		Time.timeScale = 1f;
		regularCanvas.gameObject.SetActive(true);
		pauseMenuCanvas.gameObject.SetActive(false);
		GameObject.Find("Mage").GetComponent<PlayerControl>().enabled = true;
	}

	public void DisableUI() {
		regularCanvas.gameObject.SetActive(false);
		pauseMenuCanvas.gameObject.SetActive(false);
	}

	public void GoToMenu () {
		SceneManager.LoadScene("Meniu");
	}

	public void QuitGame () {
		Application.Quit();
	}
}
