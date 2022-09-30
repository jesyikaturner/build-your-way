using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
    public enum TileType
    {
        WALK, BLOCK, BASE1, BASE2, EMPTY
    }
    public TileType Type {get; private set; }
    public int Team { get; set; }
    public Vector2 Position {get; set; }
    public bool IsSelected { get; private set; }
    public Attacker Attacker {get; private set; }

	public int xPos, zPos, playerHand = 0;
	private SpriteRenderer spriteRenderer;

    public bool isBreakable = false;
    public bool isBreaking = false;

    public Sprite walk, block, empty, base1, base2;
	public Sprite s_walk, s_block, s_base1, s_base2, s_empty;
    public Sprite b1_walk, b2_walk, b1_block, b2_block;

	// Use this for initialization
	void Start () 
    {
		spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        Type = TileType.EMPTY;
        Team = 0;
        Position = Vector2.zero;
        IsSelected = false;
	}
	
	// Update is called once per frame
	void Update () {
        if(!isBreaking)
		    SetSprite();
        if (Attacker)
        {
            isBreakable = false;
        }
	}

    public void SetType(TileType type)
    {
        Type = type;
        if(Type == TileType.BASE1) Team = 1;
        if(Type == TileType.BASE2) Team = 2;
    }

	private void SetSprite()
	{
        switch (Type)
        {
            case TileType.EMPTY:
                spriteRenderer.sprite = empty;
                break;
            case TileType.WALK:
                spriteRenderer.sprite = walk;
                break;
            case TileType.BLOCK:
                spriteRenderer.sprite = block;
                break;
            case TileType.BASE1:
                spriteRenderer.sprite = base1;
                break;
            case TileType.BASE2:
                spriteRenderer.sprite = base2;
                break;
            default:
                Debug.LogError("Sprite cannot be set!");
                break;
        }

        if (IsSelected)
        {
            if (spriteRenderer.sprite == empty)
                spriteRenderer.sprite = s_empty;
            if (spriteRenderer.sprite == walk)
                spriteRenderer.sprite = s_walk;
            if (spriteRenderer.sprite == block)
                spriteRenderer.sprite = s_block;
            if (spriteRenderer.sprite == base1)
                spriteRenderer.sprite = s_base1;
            if (spriteRenderer.sprite == base2)
                spriteRenderer.sprite = s_base2;
        }
	}

    public void ToggleSelectable()
    {
        IsSelected = !IsSelected;
    }

    // public TileInfo GetTileInfo()
    // {
    //     return _info;
    // }

	public bool SetAttacker(Attacker attacker)
	{
        // if tile has attacker return false
        if (Attacker)
        {
            Debug.LogError("Cannot set attacker.");
            return false;
        }

        // set the tile attacker to null if null is entered
        if(!attacker)
        {
            Attacker = null;
            return true;
        }

        // set the tile attacker
		Attacker = attacker;
		return true;
	}

    public int BreakAnimation(float timer)
    {
        int blockType = 0;
        if(timer > 0f)
        {
            isBreaking = true;
            if (spriteRenderer.sprite == walk)
                spriteRenderer.sprite = b1_walk;
            if (spriteRenderer.sprite == block)
                spriteRenderer.sprite = b1_block;
        }
        if (timer > 1f)
        {
            if (spriteRenderer.sprite == b1_walk)
                spriteRenderer.sprite = b2_walk;


            if (spriteRenderer.sprite == b1_block)
                spriteRenderer.sprite = b2_block;

        }
        if(timer > 2f)
        {
            if(spriteRenderer.sprite == b2_walk)
                blockType = 1;
            if (spriteRenderer.sprite == b2_block)
                blockType = 2;
            //state = PlaceState.EMPTY;
        }
        return blockType;
    }
}
