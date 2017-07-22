using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {
	// Main player controlling script

	public Slider healthBar;
	public float moveSpeed, attackRange, attackTimer = -1f, AASpeed;
	public int health, maxHealth;
	
	[System.Serializable]
	public struct Instantiable {
		public GameObject TooFarTxt, Projectile, MageSpell1, MageSpell2, MageSpell3, MageSpell4;
	} public Instantiable objects;

	[HideInInspector]
	public Vector3 hitDirection;
	[HideInInspector]
	public float moveBack, stunTime = -1f, spell1Timer = -1f, spell2Timer = -1f, spell3Timer = -1f, spell4Timer = -1f;
	[HideInInspector]
	public bool stoppedInStory;
	
	private GameObject TooFarTxt_copy, Projectile_copy, MageSpell1_copy;
	private Vector3 direction, projectileDir, SpellDir;
	private float tooFarTimer = -1f, moveSpell1 = -1f, idleTimer = -1f, transformationTimer = -1f;
	private bool moveText = false, transformed;
	private Rect HPGUI;
	private Animator anim;
	

	void Start () {
		anim = gameObject.GetComponent<Animator>();
		if(PlayerPrefs.GetInt("PASSEDOBJS" + PlayerPrefs.GetInt("SLOTNO").ToString()) == 0)
			stoppedInStory = true;
	}

	void Update () {

		// Health
		if(health <= 0f) {
			transform.gameObject.SetActive(false);
			return;
		}
		if(health > maxHealth) {
			health = maxHealth;
		}
		healthBar.value = ((float)health / maxHealth);
		healthBar.GetComponentInChildren<Text>().text = health.ToString() + "/" + maxHealth.ToString();

		//Timers
		if (tooFarTimer > 0f) tooFarTimer -= Time.deltaTime;
		else if (tooFarTimer > -1f) {
			Destroy (TooFarTxt_copy.gameObject);
			moveText = false;
		}

		if (idleTimer > 0f) {
			idleTimer -= Time.deltaTime;
			anim.SetBool("playIdle", false);
		} 

		if (attackTimer > 0f) attackTimer -= Time.deltaTime;
		if (spell1Timer > 0f) spell1Timer -= Time.deltaTime;
		if (spell2Timer > 0f) spell2Timer -= Time.deltaTime;
		if (spell3Timer > 0f) spell3Timer -= Time.deltaTime;
		if (spell4Timer > 0f) spell4Timer -= Time.deltaTime;

		if(moveSpell1 > 0f) moveSpell1 -= Time.deltaTime;
		else Destroy (MageSpell1_copy);

		//Move Objects
		if (moveSpell1 >= 0f) {
			MageSpell1_copy.GetComponent<Rigidbody>().MovePosition(MageSpell1_copy.transform.position + SpellDir * AASpeed / 2 * Time.deltaTime);
        }
		
		if (moveText) { 
			float positiony = TooFarTxt_copy.transform.position.y + (transform.position.y - TooFarTxt_copy.transform.position.y) / 3f;
			TooFarTxt_copy.transform.position = Vector3.Lerp (TooFarTxt_copy.transform.position, new Vector3 (transform.position.x, positiony, TooFarTxt_copy.transform.position.z), moveSpeed * Time.deltaTime);
		}


		// Stun
		if(stunTime > 0f || stoppedInStory) {
			anim.SetBool("isAttacking", false);
			anim.SetBool("isMoving", false);
			anim.SetBool("playIdle", false);
			anim.SetBool("tookDamage", false);
			GetComponent<Rigidbody>().velocity = Vector2.zero;
			stunTime -= Time.deltaTime;
			return;
		}
		
		// Movement direction
		if (moveBack <= 0f) {
			direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);

			if (direction != Vector3.zero) {
				float rotZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90f);
				idleTimer = Random.Range(4f, 8f);
			}
		}
			
		// Spell4 transformation control
		if(transformed) {
			if (moveBack > 0f) {
				GetComponent<Rigidbody>().velocity = new Vector2 (hitDirection.x * moveSpeed * Time.deltaTime, hitDirection.y * moveSpeed * Time.deltaTime);
				moveBack -= Time.deltaTime;
			} else if(direction != Vector3.zero) {
				direction.Normalize();
				GetComponent<Rigidbody>().velocity = new Vector2 (direction.x * moveSpeed * 6f * Time.deltaTime, direction.y * moveSpeed * 6f * Time.deltaTime);
			} else {
				GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
			}
			transformationTimer -= Time.deltaTime;
			if(transformationTimer <= 0f) {
				transformed = false;
				GameObject MageSpell4_copy= (GameObject) Instantiate (objects.MageSpell4, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation);
				MageSpell4_copy.SetActive(true);
				transform.GetChild(0).gameObject.SetActive(true);
				transform.GetChild(1).gameObject.SetActive(false);
			}
			return;
		}

		// Backstep + Moving
		anim.SetBool("tookDamage", false);
		if (moveBack > 0f) {
			anim.SetBool("tookDamage", true);
			float rotZ = Mathf.Atan2 (hitDirection.y, hitDirection.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rotZ - 90f);
			GetComponent<Rigidbody>().velocity = new Vector2 (hitDirection.x * moveSpeed * Time.deltaTime, hitDirection.y * moveSpeed * Time.deltaTime);
			moveBack -= Time.deltaTime;
		} else if(direction != Vector3.zero) {
			direction.Normalize();
			anim.SetBool("isMoving", true);
			GetComponent<Rigidbody>().velocity = new Vector2 (direction.x * moveSpeed * Time.deltaTime, direction.y * moveSpeed * Time.deltaTime);
		} else {
			anim.SetBool("isMoving", false);
			GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);

			if (idleTimer <= 0f) {
				anim.SetBool("playIdle", true);
				idleTimer = Random.Range (4f, 8f);
			}
		}


		// Attacking
		anim.SetBool("isAttacking", false);
		if (Input.GetMouseButtonDown(1)) {
			OnRightClick();
		}

		// Spells
		// 1
		if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown("[1]")) && spell1Timer <= 0f) {
			anim.SetBool("isAttacking", true);

			Vector3 rotPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 7f));
			float rotZ = Mathf.Atan2 (rotPos.y - transform.position.y, rotPos.x - transform.position.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);
			
			GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
			moveSpell1 = 0.85f;
			if (stunTime < 0.3f) stunTime = 0.3f;
			spell1Timer = 4f;

			Vector3 copyPos = new Vector3(transform.position.x + (Mathf.Cos((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), transform.position.y + (Mathf.Sin((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), 0f);
			MageSpell1_copy = (GameObject) Instantiate (objects.MageSpell1, copyPos, Quaternion.identity);

			SpellDir = Camera.main.ScreenToWorldPoint(new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 7.6f));
			SpellDir = new Vector3(SpellDir.x - transform.position.x, SpellDir.y - transform.position.y, 0f);
			SpellDir.Normalize();

		} 
		// 2
		else if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown("[2]")) && spell2Timer <= 0f) {
			Vector3 cursorPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 7f));
			Ray clickPoint = new Ray(new Vector3(cursorPos.x, cursorPos.y, -8f), new Vector3 (0, 0, 1));
			RaycastHit hitPoint;

			if (Physics.SphereCast (clickPoint, 0.5f , out hitPoint, 20f) && (hitPoint.collider.tag == "Monsters" || hitPoint.collider.tag == "Training")) {

				Vector3 rotPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 8f));
				float rotZ = Mathf.Atan2 (rotPos.y - transform.position.y, rotPos.x - transform.position.x) * Mathf.Rad2Deg;
				transform.rotation = Quaternion.Euler (0f, 0f, rotZ + 90f);

				float distance = Mathf.Sqrt ((hitPoint.transform.position.x - transform.position.x) * (hitPoint.transform.position.x - transform.position.x) + 
					(hitPoint.transform.position.y - transform.position.y) * (hitPoint.transform.position.y - transform.position.y));

				if(distance > attackRange + 0.5f) {
					createTooFarTxt();
				} else {

					anim.SetBool("isAttacking", true);

					GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
					if (stunTime < 0.3f) stunTime = 0.3f;
					spell2Timer = 15f;
					Vector3 copyPos = new Vector3(transform.position.x + (Mathf.Cos((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), transform.position.y + (Mathf.Sin((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), 0f);
					GameObject MageSpell2_copy = (GameObject) Instantiate (objects.MageSpell2, copyPos, Quaternion.identity);
					Projectile1Script mageSpell2Script = MageSpell2_copy.gameObject.GetComponent<Projectile1Script>();
					mageSpell2Script.target = hitPoint.transform.gameObject;
				}
			}
		} 
		// 3
		else if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown("[3]")) && spell3Timer <= 0f) {
			anim.SetBool("isAttacking", true);

			GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
			if (stunTime < 3f) stunTime = 3f;
			spell3Timer = 30f;
			GameObject MageFireR1= (GameObject) Instantiate (objects.MageSpell3, new Vector3(transform.position.x, transform.position.y, 0f), Quaternion.identity);
			MageFireR1.SetActive(true);
		} 
		// 4
		else if ((Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown("[4]")) && spell4Timer <= 0f) {
			transformed = true;
			transformationTimer = 0.2f;
			spell4Timer = 15f;
			GameObject MageSpell4_copy= (GameObject) Instantiate (objects.MageSpell4, new Vector3(transform.position.x, transform.position.y, 0f), transform.rotation);
			MageSpell4_copy.SetActive(true);
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(true);
		}
	}
	
	void OnCollisionEnter(Collision Monster){
		if (Monster.gameObject.tag != "Player_Skill") {
			hitDirection = new Vector3 (Monster.contacts [0].normal.x, Monster.contacts [0].normal.y, 0f);
			GetComponent<Rigidbody>().velocity = new Vector2 (0f, 0f);
			moveBack = 0.05f; 
		}
	}

	void OnRightClick()
	{
		// Cast a ray from the mouse cursors position
		Vector3 cursorPos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 7f));
		Ray clickPoint = new Ray(new Vector3(cursorPos.x, cursorPos.y, -8f), new Vector3 (0, 0, 1));
		RaycastHit hitPoint;
		float distance = 0f;

		// See if the ray collided with an object
		if (Physics.SphereCast (clickPoint, 0.5f , out hitPoint, 20f)) {
			distance = Mathf.Sqrt ((hitPoint.transform.position.x - transform.position.x) * (hitPoint.transform.position.x - transform.position.x) + 
			                       (hitPoint.transform.position.y - transform.position.y) * (hitPoint.transform.position.y - transform.position.y));
			// Make sure this object was the one that received the right-click

			if (hitPoint.collider.tag == "Monsters" || hitPoint.collider.tag == "Training") {
				if(distance <= attackRange && stunTime <= 0f && moveBack <= 0f) { 
					// Put code for the right click event
					if(attackTimer <= 0f) {
						anim.SetBool("isAttacking", true);

						Vector3 rotPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
						float rotZ = Mathf.Atan2(rotPos.y - transform.position.y, rotPos.x - transform.position.x) * Mathf.Rad2Deg;
						transform.rotation = Quaternion.Euler(0f, 0f, rotZ + 90f);

						projectileDir = new Vector3(hitPoint.transform.position.x - transform.position.x, hitPoint.transform.position.y - transform.position.y, 0f);
						projectileDir.Normalize();
						StartCoroutine(createProjectile(hitPoint.transform.gameObject, rotZ));
					}
				} else {
					createTooFarTxt();
				}
			}
		}
	}

	void createTooFarTxt() {
		if(tooFarTimer > -1f)
			Destroy(TooFarTxt_copy.gameObject);
		tooFarTimer = 1f;
		moveText = true;
		TooFarTxt_copy = (GameObject) Instantiate(objects.TooFarTxt, new Vector3(transform.position.x, transform.position.y, -3f), Quaternion.identity);
	}

	IEnumerator createProjectile (GameObject hitPoint, float rotZ) {
		if(hitPoint == null)
			yield break;

		Projectile1Script progectileScript = objects.Projectile.GetComponent<Projectile1Script>();
		attackTimer = progectileScript.playerAttackTimer;
		stunTime = 0.5f;
		yield return new WaitForSeconds (0.2f);

		if(hitPoint == null)
			yield break;

		Vector3 copyPos = new Vector3(transform.position.x + (Mathf.Cos((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), transform.position.y + (Mathf.Sin((rotZ - 90f) * Mathf.Deg2Rad) * 0.3f), 0f);
		Projectile_copy = (GameObject) Instantiate (objects.Projectile, copyPos, Quaternion.identity);
		progectileScript = Projectile_copy.GetComponent<Projectile1Script>();
		progectileScript.target = hitPoint;
	}
}


