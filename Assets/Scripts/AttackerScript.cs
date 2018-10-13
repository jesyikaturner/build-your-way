using UnityEngine;
using System.Collections;

public class AttackerScript : MonoBehaviour {

	private SpriteRenderer _curr;

	public Sprite player1_attacker, player2_attacker;
	public int team = 0;

	// Use this for initialization
	void Start () {
		_curr = transform.GetChild(0).GetComponent<SpriteRenderer>();

	}
	
	// Update is called once per frame
	void Update () {
		SetSprite();
	}

	void SetSprite()
	{
		switch (team)
		{
			case 1:
				_curr.sprite = player1_attacker;
				break;
			case 2:
				_curr.sprite = player2_attacker;
				break;
			default:
				Debug.LogError("Sprite cannot be set!");
				break;
		}
	}
}
