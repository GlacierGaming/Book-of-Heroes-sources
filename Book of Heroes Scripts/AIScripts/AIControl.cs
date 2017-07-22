using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIControl : MonoBehaviour {

	public Grid grid;
	public GameObject target;
	public Vector3 hitDirection;
	public string enemyTag;
	public float moveBack = -1f, stunTime = -1f;

	[System.Serializable]
	public struct Stats {
		public float health, hitDamage, attackSpeed, attackRange, followRange, moveSpeed;
	} public Stats stats;

	[System.Serializable]
	public struct DeathObjects {
		public GameObject greyDust, funeralStone;
	} public DeathObjects deathObjects;

	[System.Serializable]
	public struct DropRate
	{
		public float dropHpRate;
	}
	public DropRate dropRate;

	private int theEnemy, neighbourIndex;
	private bool follow = false, attack = false, hasHit;
	private float isAttacking, attackTimer = -1f, timeToMove = 0.4f, timeBetweenMove = 8f, idleTimer = -1f, timeToMoveTimer, timeBetweenMoveTimer, searchPathTimer;
	private PlayerControl playerControl;
	private AIControl targetControl;
	private Animator anim = null;
	private List<Node> neighbours;



	void Start () {
		playerControl = target.GetComponent<PlayerControl>();
		anim = gameObject.GetComponent<Animator>();
	}

	void Update () {
		// Timers
		if (attackTimer > 0f) attackTimer -= Time.deltaTime;

		if (timeBetweenMoveTimer > 0f) {
			timeBetweenMoveTimer -= Time.deltaTime;
		} else {
			neighbours = grid.GetNeighbours(grid.NodeFromWorldPoint(transform.position));
			neighbourIndex = Random.Range(0, neighbours.Count - 1);
			timeToMoveTimer = Random.Range (timeToMove * 0.75f, timeToMove * 1.25f);
		}

		if (idleTimer > 0f) {
			idleTimer -= Time.deltaTime;
			anim.SetBool("playIdle", false);
		} 

		if(searchPathTimer > 0f) {
			searchPathTimer -= Time.deltaTime;
		}

		// Health
		if (stats.health <= 0f) {
			if(gameObject.tag == "Monsters") {
				Vector3 dustPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 0.8f);
				GameObject _greyDust = (GameObject)Instantiate(deathObjects.greyDust, dustPos, Quaternion.identity);
				_greyDust.GetComponent<DestroyAfterTime>().time = 1f;
				GameObject _funeralStone = (GameObject)Instantiate(deathObjects.funeralStone, transform.position, transform.rotation);
				_funeralStone.GetComponent<DestroyAfterTime>().time = 20f;

				float _percent = Random.Range(1, 100);
				if(_percent <= dropRate.dropHpRate) {
					GameObject _hpOrb = GameObject.Find("HpOrb");
					Instantiate(_hpOrb, new Vector3(transform.position.x, transform.position.y, _hpOrb.transform.position.z), Quaternion.identity);
				}
			}
			StopCoroutine("FollowPath");
			Destroy(gameObject);
		}


		// Stun
		if (stunTime > 0f) {
			anim.SetBool("isAttacking", false);
			anim.SetBool("isMoving", false);
			anim.SetBool("playIdle", false);
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			moveBack = 0f; follow = false;
			stunTime -= Time.deltaTime;
			return;
		}

		// Finding target
		GameObject[] targets = GameObject.FindGameObjectsWithTag(enemyTag);
		float minDist = 1000f;
		for (int enemy = 0; enemy < targets.Length; ++enemy) {
			float distance = Distance (new Vector3(targets[enemy].transform.position.x - transform.position.x, targets[enemy].transform.position.y - transform.position.y, 0f));
			
			if (distance < minDist) {
				minDist = distance;
				theEnemy = enemy;
			}
		}

		// Attacking
		anim.SetBool("isAttacking", false);
		if (minDist <= stats.attackRange) {
			follow = false;
			if (attackTimer <= 0f) {
				attack = true;
				attackTimer = stats.attackSpeed;
				isAttacking = 0f;
				hasHit = false;
				if (stunTime < 0.5f) stunTime = 0.5f;
			}
			if (attack) {
				anim.SetBool("isAttacking", true);

				isAttacking += Time.deltaTime;
				if (isAttacking >= 0.4f) {
					if (!hasHit) {
						hasHit = true;
						if (targets[theEnemy].name == "Mage") {
							playerControl.health -= (int)stats.hitDamage;
							playerControl.moveBack = 0.1f;
							playerControl.hitDirection = new Vector3(targets[theEnemy].transform.position.x - transform.position.x, targets[theEnemy].transform.position.y - transform.position.y, 0f);
						}
						else {
							targetControl = targets[theEnemy].GetComponent<AIControl>();
							targetControl.stats.health -= (int)stats.hitDamage;
							targetControl.moveBack = 0.1f;
							targetControl.hitDirection = new Vector3(targets[theEnemy].transform.position.x - transform.position.x, targets[theEnemy].transform.position.y - transform.position.y, 0f);
						}
					}

					if (isAttacking >= 1.25f) {
						attack = false;
					}
				}
			}
		} else {
			if(attack) {
				anim.SetBool("isAttacking", false);
			}
			follow = false;
			if (minDist <= stats.followRange) {
				follow = true;
			}
			attack = false;
			isAttacking = 0f;
		}
		

		// Moving
		if (moveBack > 0f) {
			// Push character back
			follow = false;
			anim.SetBool("tookDamage", true);
			float rotZ = Mathf.Atan2 (hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rotZ - 90);

			moveBack -= Time.deltaTime;
			GetComponent<Rigidbody>().velocity = new Vector2 (hitDirection.x * stats.moveSpeed * Time.deltaTime, hitDirection.y * stats.moveSpeed * Time.deltaTime);

			if (moveBack <= 0f) {
				if (stunTime < 0.2f) stunTime = 0.2f;
				GetComponent<Rigidbody>().velocity = Vector2.zero;
				anim.SetBool("tookDamage", false);
				return;
			}
		} else if (follow == true) {
			// Move character to target
			if(searchPathTimer <= 0f) {
				searchPathTimer = 0.4f; 
				PathRequestManager.RequestPath(grid, transform.position, targets[theEnemy].transform.position, gameObject, OnPathFound);
			}
		} else {
			// Control idle movements when character doesn't follow target
			anim.SetBool("isMoving", false);
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			if (timeToMoveTimer > 0f && minDist > stats.attackRange) {

				timeToMoveTimer -= Time.deltaTime;
				timeBetweenMoveTimer = Random.Range (timeBetweenMove * 0.75f, timeBetweenMove * 1.25f);

				if(neighbours.Count > 0 && neighbours[neighbourIndex].walkable) { 
					anim.SetBool("isMoving", true);

					float directionX = neighbours[neighbourIndex].worldPosition.x - grid.NodeFromWorldPoint(transform.position).worldPosition.x;
					float directionY = neighbours[neighbourIndex].worldPosition.y - grid.NodeFromWorldPoint(transform.position).worldPosition.y;
					
					float rotZ = Mathf.Atan2 (directionY, directionX) * Mathf.Rad2Deg;
					transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);

					GetComponent<Rigidbody>().velocity = new Vector2 (directionX * stats.moveSpeed * Time.deltaTime, directionY * stats.moveSpeed * Time.deltaTime);
				}
			} else if (idleTimer <= 0f) {
				anim.SetBool("playIdle", true);
				idleTimer = Random.Range (4f, 8f);
			}
		}
	}

	
	Vector3[] path;
	int targetIndex;

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
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

	float Distance(Vector3 direction){
		return Mathf.Sqrt (direction.x * direction.x + direction.y * direction.y);
	}

	void OnCollisionEnter (Collision Monster){
		if (Monster.gameObject.tag != "Player_Skill" && Monster.gameObject.tag != transform.gameObject.tag) {
			hitDirection = new Vector3 (Monster.contacts [0].normal.x, Monster.contacts [0].normal.y, 0f);
			GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
			moveBack = 0.2f;
		} else if (Monster.gameObject.tag == transform.gameObject.tag) {
			if (stunTime < 0.2f) stunTime = 0.2f;
		}
	}
}
