using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadingImageScript : MonoBehaviour {

	public float opaqueTime, disappearRate;

	private bool color = true;
	private Color alpha;

	void Start () {
		if(transform.GetComponent<Text>() != null)
			alpha = transform.GetComponent<Text>().color;
		else if (transform.GetComponent<SpriteRenderer>() != null)
			alpha= transform.GetComponent<SpriteRenderer>().color;
		else if (transform.GetComponent<Image>() != null)
			alpha = transform.GetComponent<Image>().color;
		else 
			color = false;
	}

	void Update () {
		if(opaqueTime > 0f) {
			opaqueTime -= Time.deltaTime;
			return;
		}

		if(color && alpha.a > 0f) {
			alpha.a -= Time.deltaTime * disappearRate;
			if(alpha.a < 0f) 
				alpha.a = 0f;

			if(transform.GetComponent<Text>() != null)
				transform.GetComponent<Text>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
			else if (transform.GetComponent<SpriteRenderer>() != null)
				transform.GetComponent<SpriteRenderer>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
			else if (transform.GetComponent<Image>() != null)
				transform.GetComponent<Image>().color = new Color(alpha.r, alpha.g, alpha.b, alpha.a);
			
		}
	}
}
