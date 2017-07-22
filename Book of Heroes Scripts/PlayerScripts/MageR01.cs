using UnityEngine;
using System.Collections;

public class MageR01 : MonoBehaviour {
	// Control the 3rd player ability

	public GameObject MageFire02;

	private float destroyTimer = 3f;

	void Update () { 
		destroyTimer -= Time.deltaTime;
		SphereCollider Fire01Collider = transform.GetComponent<Collider>().GetComponent<SphereCollider>();;
		if (transform.name == "MageFireR01" && Fire01Collider.radius <= 2) Fire01Collider.radius = 0.4f + (3f - destroyTimer) * 1.5f;
		else if (Fire01Collider.radius <= 2) Fire01Collider.radius = 0.4f + (3f - destroyTimer) * 2f;

		if (destroyTimer <= 1f && transform.name == "MageFireR01") {
			MageFire02.SetActive(true);
			Destroy (transform.gameObject);
		}
		if (destroyTimer <= 2f && transform.name == "MageFireR02") Destroy (transform.gameObject);
	}

	void OnTriggerEnter(Collider Monster) {
		if(Monster.name == "Boss") {
			if (Monster.tag == "Monsters" && transform.name == "MageFireR01") {
				BossControl bossControl = Monster.GetComponent<BossControl>();
				bossControl.stunTime = 3.5f;
			} else if (transform.name == "MageFireR02") {
				BossControl bossControl = Monster.GetComponent<BossControl>();
				Vector3 hitDirection = GameObject.Find("Mage").transform.position - bossControl.transform.position;
				hitDirection.Normalize();
				float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) - Mathf.Deg2Rad * (bossControl.transform.eulerAngles.z);
				float dirX = Mathf.Cos(angle);
				float dirY = Mathf.Sin(angle);

				if(bossControl.isVulnerable
					|| dirX == 0 && dirY > 0 
					|| dirX > 0 && dirY > 0 && dirY / dirX > 1f / Mathf.Sqrt(3) 
					|| dirX < 0 && dirY > 0 && dirY / dirX < -1f / Mathf.Sqrt(3)) {
					bossControl.stats.health -= 5f;
				} else {
					bossControl.hitInArmor = true;
				}
			}
		} else {
			if (Monster.tag == "Monsters" && transform.name == "MageFireR01") {
				AIControl aiControl = Monster.GetComponent<AIControl>();
				aiControl.stunTime = 3.5f;
			} else if (transform.name == "MageFireR02") {
				if (Monster.tag == "Monsters") {
					AIControl aiControl = Monster.GetComponent<AIControl>();
					aiControl.stats.health -= 5f;
				} else if (Monster.gameObject.name == "TrainingDummy") {
					Monster.GetComponent<Animator>().SetBool("tookDamage", true);
					float rotZ = Mathf.Atan2 (transform.position.y - Monster.transform.position.y, transform.position.x - Monster.transform.position.x) * Mathf.Rad2Deg;
					Monster.transform.rotation = Quaternion.Euler (0f, 0f, rotZ - 180);
				}
			}
		}
	}
}
