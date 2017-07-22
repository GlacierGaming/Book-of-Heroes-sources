using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TriggerScript : MonoBehaviour {
	// Trigger collider that spawns AI and triggers story box and cinematic

	public GameObject storyBox;
	public string talker;
	public int currentLine, endLine;
	public bool playCinematic;
	public Camera mainCamera, cinematicCamera;

	[System.Serializable]
	public struct Range {
		public GameObject AI;
		public int noAI;
		public Vector2 AIXRange, AIYRange;
	} public Range soldiers, goblins, orcs, trolls;

	[System.Serializable]
	public struct SpawnArrays {
		public GameObject[] soldiers, goblins, orcs, trolls;
	}
	public SpawnArrays spawnArray;

	private StoryScript storyScript;
	private bool isTriggered, spawned, activeAI = true;
	Grid grid;

	void Start () {
		grid = GameObject.Find ("Grid").GetComponent<Grid>();
		storyScript = storyBox.GetComponent<StoryScript>();

		// Spawn inactive AI
		spawnArray.soldiers = new GameObject[soldiers.noAI];
		SpawnAI (spawnArray.soldiers, soldiers.noAI, soldiers.AIXRange, soldiers.AIYRange, soldiers.AI);

		spawnArray.goblins = new GameObject[goblins.noAI];
		SpawnAI (spawnArray.goblins, goblins.noAI, goblins.AIXRange, goblins.AIYRange, goblins.AI);

		spawnArray.orcs = new GameObject[orcs.noAI];
		SpawnAI (spawnArray.orcs, orcs.noAI, orcs.AIXRange, orcs.AIYRange, orcs.AI);

		spawnArray.trolls = new GameObject[trolls.noAI];
		SpawnAI (spawnArray.trolls, trolls.noAI, trolls.AIXRange, trolls.AIYRange, trolls.AI);
	}

	void Update () {
		if (storyScript.currentLine > currentLine) {
			if (activeAI) {
				// Activate AI
				activeAI = false;
				for (int soldierNo = 0; soldierNo < soldiers.noAI; ++soldierNo) {
					spawnArray.soldiers[soldierNo].SetActive(true);
				}
				for (int goblinNo = 0; goblinNo < goblins.noAI; ++goblinNo) {
					spawnArray.goblins[goblinNo].SetActive(true);
				}
				for (int orcNo = 0; orcNo < orcs.noAI; ++orcNo) {
					spawnArray.orcs[orcNo].SetActive(true);
				}
				for (int trollNo = 0; trollNo < trolls.noAI; ++trollNo) {
					spawnArray.trolls[trollNo].SetActive(true);
				}
			}
			if (isTriggered && playCinematic) {
				// Activate cinematic
				GameObject.Find("Mage").GetComponent<PlayerControl>().stunTime = 1000;

				mainCamera.GetComponent<Camera>().enabled = false;
				cinematicCamera.gameObject.SetActive(true);
				cinematicCamera.GetComponent<Camera>().enabled = true;
				cinematicCamera.GetComponent<Animation>().Play();
			}
		}
		if (isTriggered && storyScript.currentLine > endLine) {
			if (playCinematic) {
				GameObject.Find("Mage").GetComponent<PlayerControl>().stunTime = 0;
				mainCamera.GetComponent<Camera>().enabled = true;
				Destroy(cinematicCamera.gameObject);
			}
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter (Collider other) {
		// Trigger story box and set triggered bool
		if (other.name == "Mage") {
			if (currentLine <= endLine) {
				storyScript.isActive = true;
				storyScript.transform.GetChild(2).GetComponent<Text>().text = talker;
				storyScript.currentLine = currentLine;
				storyScript.endLine = endLine;
				storyScript.EnableDialogBox();
			}

			isTriggered = true;
		}
	}

	void SpawnAI (GameObject[] spawnAI, int noAI, Vector2 AIXRange, Vector2 AIYRange, GameObject AIToSpawn) {
		for (int AINo = 0; AINo < noAI; ++AINo) {
			bool isSpawned = false;
			while (!isSpawned) {
				float x = Random.Range (AIXRange.x, AIXRange.y), y = Random.Range (AIYRange.x, AIYRange.y);
				Vector3 randPos = new Vector3 (x, y, 0f);
				
				if (grid.NodeFromWorldPoint(randPos).walkable) {
					isSpawned = true;
					spawnAI[(int)AINo] = ((GameObject) Instantiate(AIToSpawn, randPos, Quaternion.identity));
					spawnAI[(int)AINo].SetActive(false);
				}
			}
		}
	}
}
