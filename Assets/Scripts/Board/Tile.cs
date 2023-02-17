using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour 
{
    public enum TileStatus
    {
        EMPTY, 
        WALK, 
        BLOCK, 
        BASE1, 
        BASE2
    }
    public TileStatus Status { get; private set; }
    public int Team { get; set; }
    public Vector2 Position {get; set; }
    public bool IsSelected { get; private set; }
    public Attacker Attacker {get; private set; }

	public int xPos, zPos, playerHand = 0;
	public SpriteRenderer spriteRenderer;

    public bool isBreakable = false;
    public bool isBreaking = false;

    public Sprite walk, block, empty, base1, base2;
	public Sprite s_walk, s_block, s_base1, s_base2, s_empty;
    public Sprite b1_walk, b2_walk, b1_block, b2_block;


	void Start () 
    {
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
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

    public void SetStatus(TileStatus inputStatus)
    {
        Status = inputStatus;
        if(Status == TileStatus.BASE1) Team = 1;
        if(Status == TileStatus.BASE2) Team = 2;

    }

	private void SetSprite()
	{
        Sprite currSprite = spriteRenderer.sprite;

        switch (Status)
        {
            case TileStatus.EMPTY:
                spriteRenderer.sprite = empty;
                break;
            case TileStatus.WALK:
                spriteRenderer.sprite = walk;
                break;
            case TileStatus.BLOCK:
                spriteRenderer.sprite = block;
                break;
            case TileStatus.BASE1:
                spriteRenderer.sprite = base1;
                break;
            case TileStatus.BASE2:
                spriteRenderer.sprite = base2;
                break;
            default:
                Debug.LogError("Sprite cannot be set!");
                break;
        }

        if (IsSelected)
        {
            if (spriteRenderer.sprite == empty) spriteRenderer.sprite = s_empty;
            if (spriteRenderer.sprite == walk) spriteRenderer.sprite = s_walk;
            if (spriteRenderer.sprite == block) spriteRenderer.sprite = s_block;
            if (spriteRenderer.sprite == base1) spriteRenderer.sprite = s_base1;
            if (spriteRenderer.sprite == base2) spriteRenderer.sprite = s_base2;
        }
	}

    public void ToggleSelectable()
    {
        IsSelected = !IsSelected;
    }

	public bool SetAttacker(Attacker attacker)
	{
        // set the tile attacker to null if null is entered
        if(attacker == null)
        {
            Attacker = null;
            return true;
        }
        // if tile has attacker return false
        if (Attacker)
        {
            Debug.LogError("Cannot set attacker.");
            return false;
        }

        // set the tile attacker
		Attacker = attacker;
		return true;
	}

    public int BreakAnimation(float timer)
    {
        int blockStatus = 0;
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
                blockStatus = 1;
            if (spriteRenderer.sprite == b2_block)
                blockStatus = 2;
            //state = PlaceState.EMPTY;
        }
        return blockStatus;
    }
}
