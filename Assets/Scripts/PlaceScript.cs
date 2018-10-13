using UnityEngine;
using System.Collections;

public class PlaceScript : MonoBehaviour 
{
	public int xPos, zPos, playerHand = 0, team = 0;
	public AttackerScript _attacker;
	private SpriteRenderer _curr;

	/*
	 * -4 selected base2 + -3 selected base1 + -2 selected block + -1 selected tile
	 * 0 no tile + 1 tile + 2 block + 3 base1 + 4 base2
	 */ 
	public int blockType = 0; 
	public bool isWalkable = false;
    public bool isBreakable = false;

	public Sprite walk, block, empty, base1, base2;
	public Sprite s_walk, s_block, s_base1, s_base2, s_empty;
    public Sprite b1_walk, b2_walk, b1_block, b2_block;

	// Use this for initialization
	void Start () {
		_curr = transform.GetChild(0).GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		SetSprite();
	}

    // like this is awful but it requires such a massive rewrite so meh.
    // sets the sprite based on the blocktype.
	private void SetSprite()
	{
		switch(blockType)
		{
        case -5:
                _curr.sprite = s_empty;
            break;
		case -4:
			    _curr.sprite = s_base2;
			break;
		case -3:
			    _curr.sprite = s_base1;
			break;
		case -2:
			    _curr.sprite = s_block;
			break;
		case -1:
			    _curr.sprite = s_walk;
			break;
		case 0:
			    _curr.sprite = empty;
			break;
		case 1:
			    _curr.sprite = walk;
			break;
		case 2:
                _curr.sprite = block;
			break;
		case 3:
			    _curr.sprite = base1;
			break;
		case 4:
			    _curr.sprite = base2;
			break;
        case 5:
                _curr.sprite = b1_walk;
            break;
        case 6:
                _curr.sprite = b2_walk;
            break;
        case 7:
                _curr.sprite = b1_block;
            break;
        case 8:
                _curr.sprite = b2_block;
            break;


		default:
			Debug.LogError("Sprite cannot be set!");
			break;
		}
	}

	public bool SetAttacker(AttackerScript attacker)
	{
        if(attacker == null)
        {
            _attacker = null;
            return true;
        }

		if(_attacker)
		{
			Debug.LogError("Cannot set attacker.");
			return false;
		}

		_attacker = attacker;
		return true;
	}

	public AttackerScript GetAttacker()
	{
		return _attacker;
	}
}
