using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCanvas : MonoBehaviour {

	public GameObject mainCanvas, secondCanvas;

	public void Switch (int targetCanvas) {
		if(targetCanvas == 1) {
			secondCanvas.SetActive(false);
			mainCanvas.SetActive(true);
		} else {
			mainCanvas.SetActive(false);
			secondCanvas.SetActive(true);
		}
	}
}
