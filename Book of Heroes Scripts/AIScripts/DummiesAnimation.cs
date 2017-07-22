using UnityEngine;
using System.Collections;

public class DummiesAnimation : MonoBehaviour {
	// Make the training soldiers attack with a random time delay between each attack

	private float animTimer;

	void Start () {
		animTimer = Random.Range (1f, 6f);
	}

	void Update () {
		if (GetComponent<Animator>().GetBool("tookDamage") == true) {
			StartCoroutine(setAnimFalse());
		}
		if (animTimer > 0f) {
			animTimer -= Time.deltaTime;
			gameObject.GetComponent<Animator>().SetBool("Idle", false);
		} else {
			animTimer = Random.Range (5f, 10f);
			gameObject.GetComponent<Animator>().SetBool("Idle", true);
		}
	}

	IEnumerator setAnimFalse () {
		yield return new WaitForSeconds(0.1f);
		GetComponent<Animator>().SetBool("tookDamage", false);
	}
}
