using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoryScript : MonoBehaviour {
	// Story control

	public GameObject tutorialCollider;
	public TextAsset textFile;
	public int currentLine, endLine;
	public bool isActive;

	private string[] textLines;

	void Start () {
		if (textFile != null) {
			textLines = textFile.text.Split('\n');
		}

		if (isActive) {
			EnableDialogBox();
		} else {
			DisableDialogBox();
		}
	}

	void Update () {
		if (currentLine <= 25 && currentLine > 2) StartCoroutine(TellStory());
		if (currentLine > 25 && PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString()) == 0) 
			tutorialCollider.SetActive(false);

		if (!isActive) {
			return;
		}


		if(currentLine == 65) 
			transform.GetChild(2).GetComponent<Text>().text = "Erimoth";

		transform.GetChild(1).GetComponent<Text>().text = textLines[currentLine].Replace("\\n", "\n");

		if (currentLine <= 25 && currentLine == endLine) {
			// Let the player move when we ask him to do something in the tutorial
			GameObject.Find("Mage").GetComponent<PlayerControl>().stoppedInStory = false;
		}
		if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !GameObject.Find("Cinematic")) {
			if (currentLine <= 25 && currentLine == endLine) {
				// The part where we wait for the player to do what we ask in the tutorial
				currentLine -= 1;
			}
			currentLine += 1;
		}

		if (currentLine > endLine) {
			DisableDialogBox();
		}
	}

	public void EnableDialogBox() {
		GameObject.Find("Mage").GetComponent<PlayerControl>().stoppedInStory = true;
		GameObject.Find("Boss").GetComponent<BossControl>().stoppedInStory = true;
		isActive = true;
		for(int childNo = 0; childNo < transform.childCount; ++childNo)
			transform.GetChild(childNo).gameObject.SetActive(true);
	}

	public void DisableDialogBox() {
		GameObject.Find("Mage").GetComponent<PlayerControl>().stoppedInStory = false;
		GameObject.Find("Boss").GetComponent<BossControl>().stoppedInStory = false;
		isActive = false;
		for(int childNo = 0; childNo < transform.childCount; ++childNo)
			transform.GetChild(childNo).gameObject.SetActive(false);
	}

	IEnumerator TellStory() {
		// Tutorial part
		if (currentLine == 25 && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown("[4]"))) {
			DisableDialogBox();
			currentLine = 28;
			endLine = 32;
			yield return new WaitForSeconds(0.5f);
			EnableDialogBox();
		} else if (currentLine == 21 && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown("[3]"))) {
			DisableDialogBox();
			currentLine = 24;
			endLine = 25;
			yield return new WaitForSeconds(3f);
			EnableDialogBox();
		} else if (currentLine == 17 && GameObject.Find("MageFire2(Clone)")) {
			DisableDialogBox();
			currentLine = 20;
			endLine = 21;
			yield return new WaitForSeconds(0.5f);
			EnableDialogBox();
		} else if (currentLine == 12 && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("[1]"))) {
			DisableDialogBox();
			currentLine = 15;
			endLine = 17;
			yield return new WaitForSeconds(0.5f);
			EnableDialogBox();
		} else if (currentLine == 7 && GameObject.Find("Projectile(Clone)")) {
			DisableDialogBox();
			currentLine = 10;
			endLine = 12;
			yield return new WaitForSeconds(0.5f);
			EnableDialogBox();
		} else if (currentLine == 3 && (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))) {
			DisableDialogBox();
			currentLine = 6;
			endLine = 7;
			yield return new WaitForSeconds(0.5f);
			EnableDialogBox();
		}
	}
}
