using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpellBarScript : MonoBehaviour {

	public GameObject player;

	[System.Serializable]
	public struct Spell {
		public Image icon, usedIcon;
		public Text CD;
	} public Spell Spell1, Spell2, Spell3, Spell4;

	private PlayerControl playerControl;

	void Start () {
		playerControl = player.GetComponent<PlayerControl>();
	}

	void Update () {
		Spell1.CD.text = ((float) ((int) (playerControl.spell1Timer * 10)) / 10).ToString();
		Spell2.CD.text = ((float) ((int) (playerControl.spell2Timer * 10)) / 10).ToString();
		Spell3.CD.text = ((float) ((int) (playerControl.spell3Timer * 10)) / 10).ToString();
		Spell4.CD.text = ((float) ((int) (playerControl.spell4Timer * 10)) / 10).ToString();

		Timer (playerControl.spell1Timer, Spell1.icon, Spell1.usedIcon);
		Timer (playerControl.spell2Timer, Spell2.icon, Spell2.usedIcon);
		Timer (playerControl.spell3Timer, Spell3.icon, Spell3.usedIcon);
		Timer (playerControl.spell4Timer, Spell4.icon, Spell4.usedIcon);
	}

	void Timer (float spellTimer, Image Spell, Image SpellUsed) {
		if (spellTimer > 0f && Spell.gameObject.activeSelf) {
			Spell.gameObject.SetActive (false);
			SpellUsed.gameObject.SetActive (true);
		} else if (spellTimer <= 0f && Spell.gameObject.activeSelf == false) {
			Spell.gameObject.SetActive (true);
			SpellUsed.gameObject.SetActive (false);
		}
	}
}
