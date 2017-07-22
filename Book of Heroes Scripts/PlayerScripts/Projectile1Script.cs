using UnityEngine;
using System.Collections;

public class Projectile1Script : MonoBehaviour {
	// Control all player projectiles

	public GameObject Player, target, FireHitParticle, stunParticle;
	public float playerAttackTimer, damage, moveSpeed;

	private AIControl aiControl;
	private PlayerControl playerControl;
	BossControl bossControl;

	void Start () {
		playerControl = Player.gameObject.GetComponent<PlayerControl>();
		if (name != "Projectile(Clone)") playerControl.attackTimer = playerAttackTimer;
	}

	void Update () {
		if (transform.gameObject.name == "Projectile(Clone)" || transform.gameObject.name == "MageFire2(Clone)") {
			if (target == null) {
				Destroy (gameObject);
				return;
			}
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (target.transform.position.x, target.transform.position.y, 0f), moveSpeed * Time.deltaTime);

			if (Vector2.Distance(target.transform.position, transform.position) <= 0f) {

				if (target.gameObject.tag == "Monsters") {
					HitEnemy (target.GetComponent<Collider>());
				} else if(target.name == "TrainingDummy") {
					if(target.name != "Boss") {
						target.GetComponent<Animator>().SetBool("tookDamage", true);
					}
					float rotZ = Mathf.Atan2 (Player.transform.position.y - target.transform.position.y, Player.transform.position.x - target.transform.position.x) * Mathf.Rad2Deg;
					target.transform.rotation = Quaternion.Euler (0f, 0f, rotZ - 180);
				}
				if (transform.gameObject.name == "MageFire2(Clone)") {
					if (target.gameObject.tag == "Monsters" && target.name != "Boss") aiControl.stunTime = 1f;

					// Make the fire cage effect around the target
					CapsuleCollider targetCollider = target.GetComponent<Collider>().GetComponent<CapsuleCollider>();
					ParticleColumn (0f, targetCollider.radius * 2);
					ParticleColumn (0f, -targetCollider.radius * 2);
					ParticleColumn (targetCollider.radius * 2, 0f);
					ParticleColumn (-targetCollider.radius * 2, 0f);
					ParticleColumn (targetCollider.radius * 1.5f, targetCollider.radius * 1.5f);
					ParticleColumn (-targetCollider.radius * 1.5f, -targetCollider.radius * 1.5f);
					ParticleColumn (targetCollider.radius * 1.5f, -targetCollider.radius * 1.5f);
					ParticleColumn (-targetCollider.radius * 1.5f, targetCollider.radius * 1.5f);
				}
				Destroy (transform.gameObject);
			}
		}
	}

	void OnTriggerEnter(Collider Monster) {
		if (transform.name == "MageFire1(Clone)"){
			if (Monster.gameObject.tag == "Monsters") {
				HitEnemy (Monster);
			} else if (Monster.gameObject.name == "TrainingDummy") {
				if(Monster.name != "Boss") {
					Monster.GetComponent<Animator>().SetBool("tookDamage", true);
				}
				float rotZ = Mathf.Atan2 (Player.transform.position.y - Monster.transform.position.y, Player.transform.position.x - Monster.transform.position.x) * Mathf.Rad2Deg;
				Monster.transform.rotation = Quaternion.Euler (0f, 0f, rotZ - 180);
			}
		}
	}

	void HitEnemy(Collider Monster) {
		if(Monster.name == "Boss") {
			bossControl = Monster.GetComponent<BossControl>();
			Vector3 hitDirection = playerControl.transform.position - bossControl.transform.position;
			hitDirection.Normalize();
			float angle = Mathf.Atan2(hitDirection.y, hitDirection.x) - Mathf.Deg2Rad * (bossControl.transform.eulerAngles.z);
			float dirX = Mathf.Cos(angle);
			float dirY = Mathf.Sin(angle);

			if(bossControl.isVulnerable
				|| dirX == 0 && dirY > 0 
				|| dirX > 0 && dirY > 0 && dirY / dirX > 1f / Mathf.Sqrt(3) 
				|| dirX < 0 && dirY > 0 && dirY / dirX < -1f / Mathf.Sqrt(3)) {
				bossControl.stats.health -= damage;
			} else {
				bossControl.hitInArmor = true;
			}
		} else {
			aiControl = Monster.gameObject.GetComponent<AIControl> ();
			if (transform.name != "MageFire2(Clone)") aiControl.moveBack = 0.08f;
			aiControl.hitDirection = aiControl.transform.position - playerControl.transform.position;
			aiControl.stats.health -= damage;
		}
		Instantiate(FireHitParticle, new Vector3(transform.position.x, transform.position.y, -0.8f), transform.rotation);
	}

	void ParticleColumn (float Distx, float Disty) {
		Vector3 particleEffect01 = new Vector3 (target.transform.position.x + Distx, target.transform.position.y + Disty, 0f);
		Instantiate (stunParticle, particleEffect01, stunParticle.transform.rotation);
	}
}
