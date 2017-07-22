using UnityEngine;
using System.Collections;

public class DestroyAfterTime : MonoBehaviour {
	// Destroy object after a set time

	public float time;

	void Update () {
		if(time > 0f)
			time -= Time.deltaTime;
		else
			Destroy(gameObject);
	}
}
