using UnityEngine;
using System.Collections;

public class ConsumableLoot : MonoBehaviour {

	public GameObject endEffect;
	public string variable;
	public int value;

	private PlayerControl playerControl;

	void Start () {
		if(gameObject.name == "HpOrb(Clone)") {
			gameObject.GetComponent<DestroyAfterTime>().enabled = true;
			gameObject.GetComponent<DestroyAfterTime>().time = 15f;
		} else {
			gameObject.GetComponent<DestroyAfterTime>().enabled = false;
		}
	}

	void OnTriggerEnter(Collider Other) {
		if (Other.gameObject.name == "Mage"){
			
			playerControl = Other.gameObject.GetComponent<PlayerControl>();

			switch(variable) {
				case "health" : if(playerControl.health < playerControl.maxHealth) { 
									playerControl.health += value; 
								}
								break;
			}

			GameObject _endEffect = (GameObject)Instantiate(endEffect, transform.position, Quaternion.identity);
			_endEffect.GetComponent<DestroyAfterTime>().time = 1f;
			Destroy(gameObject);
		}
	}
}
