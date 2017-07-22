using UnityEngine;
using System.Collections;

public class TrainingSoldierScript : MonoBehaviour {

	private float startTimer;

	void Start () {
		startTimer = Random.Range(0f, 1f);
	}

	void Update () {
		startTimer -= Time.deltaTime;
		if (startTimer <= 0f) {
			gameObject.GetComponent<Animator>().SetBool("isAttacking", true);
		}
	}
}
