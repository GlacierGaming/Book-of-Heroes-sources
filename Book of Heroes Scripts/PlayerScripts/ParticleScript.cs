using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {
	// Destroy object after some time

	private float lifeTime = 1f;

	void Update () {
		lifeTime -= Time.deltaTime;
		if (lifeTime <= 0f) Destroy (transform.gameObject);
	}
}
