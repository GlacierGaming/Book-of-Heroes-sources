using UnityEngine;
using System.Collections;

public class MageWScript : MonoBehaviour {
	// Control player healing ability (not in game atm)

	public GameObject target;

	private GameObject FireWChild;
	private float destroyTimer = 1.5f;
	private bool plusHP = false, gaveHP = false;
	private PlayerControl playerControl;

	void Start () {
		playerControl = target.GetComponent<PlayerControl>();
		if (playerControl.health < playerControl.maxHealth - 1) playerControl.health += 1;
		else playerControl.health = playerControl.maxHealth;
	}

	void Update () {
		destroyTimer -= Time.deltaTime;
		if (destroyTimer <= 1f) plusHP = true;

		if (plusHP == true && gaveHP == false) {
			playerControl = target.GetComponent<PlayerControl>();
			if (playerControl.health < playerControl.maxHealth - 1) playerControl.health += 1;
			else playerControl.health = playerControl.maxHealth;
			gaveHP = true;
		}

		if (destroyTimer <= 0f) Destroy (transform.gameObject);

		FireWChild = transform.GetChild(2).gameObject;
		FireWChild.transform.position = new Vector3(transform.position.x, transform.position.y, destroyTimer - 2f);
	}
}
