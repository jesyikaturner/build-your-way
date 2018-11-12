using UnityEngine;
using System.Collections;

public class Attacker : MonoBehaviour {

	private SpriteRenderer sprite;

	public Sprite player1_attacker, player2_attacker;
    public Sprite s_player1_attacker, s_player2_attacker;
	public int team = 0;
    private int attackerType = 0;

	// Use this for initialization
	void Start () {
		sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        attackerType = team;
	}
	
	// Update is called once per frame
	void Update () {
		SetSprite();
	}

	void SetSprite()
	{
		switch (attackerType)
		{
            case -2:
                sprite.sprite = s_player2_attacker;
                break;
            case -1:
                sprite.sprite = s_player1_attacker;
                break;
			case 1:
				sprite.sprite = player1_attacker;
				break;
			case 2:
				sprite.sprite = player2_attacker;
				break;
			default:
				Debug.LogError("Sprite cannot be set!");
				break;
		}
	}

    public void ToggleSelect()
    {
        attackerType *= -1;
    }
}
