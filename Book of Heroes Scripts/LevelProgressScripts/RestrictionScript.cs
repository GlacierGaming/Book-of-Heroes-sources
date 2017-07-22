using UnityEngine;
using System.Collections;

public class RestrictionScript : MonoBehaviour {

	public GameObject restrictionBox, triggerCollider, storyBox;
	public bool destroyWhenNoEnemies;
	
	private GameObject[] spawnedGoblins, spawnedOrcs, spawnedTrolls;
	private bool isActive, storyBoxWasActive, collectedEnemies;

	void Update () {
		if(!collectedEnemies) {
			// Put the enemies in the specific area in arrays
			collectedEnemies = true;
			TriggerScript triggerScript = triggerCollider.GetComponent<TriggerScript>();
			spawnedGoblins = new GameObject[triggerScript.goblins.noAI];
			for (int goblinNo = 0; goblinNo < triggerScript.goblins.noAI; ++goblinNo) {
				spawnedGoblins[goblinNo] = triggerScript.spawnArray.goblins[goblinNo];
			}
			spawnedOrcs = new GameObject[triggerScript.orcs.noAI];
			for (int orcNo = 0; orcNo < triggerScript.orcs.noAI; ++orcNo) {
				spawnedOrcs[orcNo] = triggerScript.spawnArray.orcs[orcNo];
			}
			spawnedTrolls = new GameObject[triggerScript.trolls.noAI];
			for (int trollNo = 0; trollNo < triggerScript.trolls.noAI; ++trollNo) {
				spawnedTrolls[trollNo] = triggerScript.spawnArray.trolls[trollNo];
			}
		}


		if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonUp(0)) && isActive) {
			// Restriction box control
			isActive = false;
			restrictionBox.SetActive(false);
			if(storyBoxWasActive) {
				storyBox.SetActive(true);
				storyBoxWasActive = false;
			}
			GameObject.Find("Mage").GetComponent<PlayerControl>().stunTime = 0;
		}

		if (destroyWhenNoEnemies) { 
			// Destroy the object when no enemies are active if the bool is true
			bool existEnemies = false;
			for (int goblinNo = 0; goblinNo < spawnedGoblins.Length; ++goblinNo) 
				if (spawnedGoblins[goblinNo]) {
					existEnemies = true;
				}
			for (int orcNo = 0; orcNo < spawnedOrcs.Length; ++orcNo) 
				if (spawnedOrcs[orcNo] != null) {
					existEnemies = true;
				}
			for(int trollNo = 0; trollNo < spawnedTrolls.Length; ++trollNo) 
				if (spawnedTrolls[trollNo] != null) {
					existEnemies = true;
				}
			if (!existEnemies) {
				Destroy(gameObject);
			}
		}
	}

	void OnCollisionEnter (Collision other) {
		// Activate restriction box
		if (other.gameObject.name == "Mage") {
			if(storyBox.activeSelf == true) {
				storyBox.SetActive(false);
				storyBoxWasActive = true;
			}
			isActive = true;
			restrictionBox.SetActive(true);
			other.gameObject.GetComponent<PlayerControl>().stunTime = 1000;
		}
	}
}
