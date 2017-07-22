using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BossControl : MonoBehaviour {

	public Grid grid;
	public GameObject target, storyBox;
	public GameObject[] armor, fallingArmor;
	public Vector3 hitDirection;
	public float stunTime = -1f;

	[HideInInspector]
	public bool isVulnerable, stoppedInStory, hitInArmor;

	[System.Serializable]
	public struct Stats {
		public float health, hitDamage, attackSpeed, attackRange, jumpRange, followRange, moveSpeed;
	} public Stats stats;

	[System.Serializable]
	public struct DeathObjects {
		public GameObject greyDust, funeralStone;
	} public DeathObjects deathObjects;

	bool follow, attack, hasHit, isJumping, triggeredDialog;
	float isAttacking, attackTimer, searchPathTimer, jumpTimer, isOccupied, pullingTimer, preJump, stopForAttack, theEnd;
	Vector3 jumpTarget;
	PlayerControl playerControl;
	StoryScript storyScript;
	Animator anim = null;


	void Start () {
		playerControl = target.GetComponent<PlayerControl>();
		anim = gameObject.GetComponent<Animator>();
		storyScript = storyBox.GetComponent<StoryScript>();
	}

	void Update () {

		if (hitInArmor && !triggeredDialog) {
			triggeredDialog = true;
			storyScript.isActive = true;
			storyScript.transform.GetChild(2).GetComponent<Text>().text = "Bash'Than";
			storyScript.currentLine = 64;
			storyScript.endLine = 67;
			storyScript.EnableDialogBox();
		}

		// Timers

		if (theEnd > 0f) {
			theEnd -= Time.deltaTime;
			if (theEnd <= 0f) {
				SceneManager.LoadScene("Meniu");
			}
			return;
		} 

		if (attackTimer > 0f) {
			attackTimer -= Time.deltaTime;
		}

		if (searchPathTimer > 0f) {
			searchPathTimer -= Time.deltaTime;
		}

		if (jumpTimer > 0f) {
			jumpTimer -= Time.deltaTime;
		}

		if (isOccupied > 0f) {
			isOccupied -= Time.deltaTime;
		}

		if (pullingTimer > 0f) {
			StopCoroutine("FollowPath");
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			pullingTimer -= Time.deltaTime;

			if(pullingTimer <= 1f) {
				anim.SetBool("PullWeapon", true);
				isJumping = false;
			}
		} else {
			anim.SetBool("PullWeapon", false);
		}

		// Health
		if (stats.health <= 0f) {
			if(gameObject.tag == "Monsters") {
				Vector3 dustPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.8f);
				GameObject _greyDust = (GameObject)Instantiate(deathObjects.greyDust, dustPos, Quaternion.identity);
				_greyDust.GetComponent<DestroyAfterTime>().time = 1f;
				GameObject _funeralStone = (GameObject)Instantiate(deathObjects.funeralStone, transform.position, transform.rotation);
				_funeralStone.GetComponent<DestroyAfterTime>().time = 20f;
			}
			StopCoroutine("FollowPath");
			playerControl.stunTime = 10f;
			transform.GetChild(0).gameObject.SetActive(false);
			theEnd = 2f;
		}
		if(stats.health <= 30 && armor[0]) {
			Destroy(armor[0]);
			fallingArmor[0].transform.position = transform.position;
			fallingArmor[0].transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
			fallingArmor[0].SetActive(true);
		}
		if(stats.health <= 20 && armor[1]) {
			Destroy(armor[1]);
			fallingArmor[1].transform.position = transform.position;
			fallingArmor[1].transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
			fallingArmor[1].SetActive(true);
		}
		if(stats.health <= 10 && armor[2]) {
			isVulnerable = true;
			Destroy(armor[2]);
			Destroy(armor[3]);
			fallingArmor[2].transform.position = transform.position;
			fallingArmor[2].transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);
			fallingArmor[2].SetActive(true);
		}

		// Stun
		if (stunTime > 0f || stoppedInStory) {
			StopCoroutine("FollowPath");
			anim.SetBool("isAttacking", false);
			anim.SetBool("isMoving", false);
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			follow = false;
			stunTime -= Time.deltaTime;
			return;
		}

		if (preJump > 0f) {
			StopCoroutine("FollowPath");
			anim.SetBool("isAttacking", false);
			anim.SetBool("isMoving", false);
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			follow = false;
			preJump -= Time.deltaTime;
			return;
		}

		// Attacking
		//Melee Attack
		anim.SetBool("isAttacking", false);
		if (pullingTimer <= 0f && !isJumping && Vector3.Distance(transform.position, target.transform.position) <= stats.attackRange) {
			
			if (attackTimer <= 0f) {
				attack = true;
				attackTimer = stats.attackSpeed;
				isAttacking = 0f;
				hasHit = false;
				stopForAttack = 1.5f;

				float rotZ = Mathf.Atan2 (target.transform.position.y - transform.position.y, target.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);
			}
			if (attack) {
				isOccupied = Mathf.Max(isOccupied, 1f);
				anim.SetBool("isAttacking", true);

				isAttacking += Time.deltaTime;
				if (isAttacking >= 0f) {
					if (!hasHit) {
						hasHit = true;
						playerControl.health -= (int)stats.hitDamage;
						playerControl.moveBack = 0.3f;
						playerControl.hitDirection = new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, 0f);
					}

					if (isAttacking >= 1f) {
						attack = false;
					}
				}
			}
		} else if(!isJumping) {

			//Jump Attack
			anim.SetBool("isJumping", false);
			if (!isJumping && isOccupied <= 0f && jumpTimer <= 0f && Vector3.Distance(transform.position, target.transform.position) <= stats.jumpRange) {
				isJumping = true;

				if(!isJumping) {
					preJump = 1f;
					return;
				}

				jumpTarget = target.transform.position;
				jumpTimer = 15f;
				anim.SetBool("isJumping", true);

				float rotZ = Mathf.Atan2 (jumpTarget.y - transform.position.y, jumpTarget.x - transform.position.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);

			}
		}


		if (stopForAttack > 0f) {
			StopCoroutine("FollowPath");
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			follow = false;
			stopForAttack -= Time.deltaTime;
			return;
		}


		if(isJumping) { 
			StopCoroutine("FollowPath");

			Vector3 direction = new Vector3 (jumpTarget.x - transform.position.x, jumpTarget.y - transform.position.y, 0f);
			direction.Normalize ();
			GetComponent<Rigidbody>().velocity = new Vector2 (direction.x * 400f * Time.deltaTime, direction.y * 400f * Time.deltaTime);

			if(Vector3.Distance(jumpTarget, transform.position) <= 1f) {
				if(pullingTimer <= 0f) {
					pullingTimer = 8f;
				}
				GetComponent<Rigidbody>().velocity = Vector2.zero;
				StopCoroutine("FollowPath");
			}
			return;
		}

		if(pullingTimer > 0f) {
			StopCoroutine("FollowPath");
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			follow = false;
			return;
		}

		// Moving
		follow = false;
		if (Vector3.Distance(transform.position, target.transform.position) <= stats.followRange && !isJumping) {
			follow = true;
		}


		if (follow == true) {
			if(searchPathTimer <= 0f) {
				searchPathTimer = 0.4f; 
				PathRequestManager.RequestPath(grid, transform.position, target.transform.position, gameObject, OnPathFound);
			}
		} else {
			StopCoroutine("FollowPath");
			GetComponent<Rigidbody>().velocity = Vector2.zero;
		}

	}

	Vector3[] path;
	int targetIndex;

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if(pullingTimer > 0f)
			return;
		if(pathSuccessful) {
			searchPathTimer = 0f;
			path = newPath;

			StopCoroutine("FollowPath");
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			if(newPath.Length == 0)
				return;  
			targetIndex = 0;
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath() {
		Vector3 currentWaypoint = path[0];
		while(true) {
			if(transform.position == currentWaypoint) {
				targetIndex++; Debug.Log (targetIndex);
				if(targetIndex >= path.Length) {
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			if(follow == true && Distance(new Vector3(transform.position.x - currentWaypoint.x, transform.position.y - currentWaypoint.y, 0f)) > 0.05f) {
				anim.SetBool("isMoving", true);

				float rotZ = Mathf.Atan2 (currentWaypoint.y - transform.position.y, currentWaypoint.x - transform.position.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);

				Vector3 direction = new Vector3 (currentWaypoint.x - transform.position.x, currentWaypoint.y - transform.position.y, 0f);
				direction.Normalize ();
				GetComponent<Rigidbody>().velocity = new Vector2 (direction.x * stats.moveSpeed * Time.deltaTime, direction.y * stats.moveSpeed * Time.deltaTime);
			} else {
				GetComponent<Rigidbody>().velocity = Vector2.zero;
			}
			yield return null;
		}
	}

	float Distance(Vector3 direction) {
		return Mathf.Sqrt (direction.x * direction.x + direction.y * direction.y);
	}

	void OnTriggerEnter(Collider other) {
		if(isJumping && pullingTimer <= 0f && other.gameObject.name == "Mage") {
			playerControl.health -= (int)stats.hitDamage;
			playerControl.moveBack = 0.3f;
			playerControl.hitDirection = new Vector3(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y, 0f);
		}
	}
}
