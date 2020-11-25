using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    private const int PLAYER_HAND_SIZE = 3;
    private const int MAX_BLOCK_CHANCE = 20;
    private const float TILE_SPACING = 1f;

    public Tile[] playerHand;
    public int currBlockChance;
    public int blockCounter = 0;
    private string[] nameArray = { "PLAYER 1 HAND", "PLAYER 2 HAND" };
    private List<Tile> tileDeck;

    public void PopulatePlayerHand(ref List<Tile> tileDeck, int[,] coord, Tile placement, int offset, int playerID)
    {
        this.tileDeck = tileDeck;
        playerHand = new Tile[PLAYER_HAND_SIZE];
        currBlockChance = MAX_BLOCK_CHANCE;

        if (coord.GetLength(0) > PLAYER_HAND_SIZE)
        {
            Debug.LogError("coord length entered for player hand is greather than hand size");
            return;
        }

        for (int i = 0; i < coord.GetLength(0); i++)
        {
            Tile handTile = Instantiate(placement, new Vector3(coord[i, 0] + offset * TILE_SPACING, 0, coord[i, 1] * TILE_SPACING), Quaternion.identity);
            handTile.SetState("EMPTY");
            handTile.name = nameArray[playerID-1];
            handTile.playerHand = playerID;
            playerHand[i] = handTile;
        }
    }

    public void FillHand()
    {
        //CheckTopThree();
        foreach (Tile tile in playerHand)
        {
            if (tile.state == Tile.TileState.EMPTY)
            {
                tile.state = DrawTile().GetState();
            }
        }
    }

    private void CheckTopThree()
    {
        // making sure the hand is empty
        int count = 0;
        foreach (Tile tile in playerHand)
        {
            if (tile.state == Tile.TileState.EMPTY)
            {
                count++;
            }
            else
                count = 0;
        }
        // if its not stop executing the method
        if (count > 0)
            return;

        // now to check top 3 of deck
        Tile[] tempTileArray = new Tile[3];
        int x = 0;
        for(int i = tileDeck.Count; i > tileDeck.Count - 3; i--)
        {
            tempTileArray[x] = tileDeck[i];
            x++;
        }

        count = 0;

        // checking if all 3 are block tiles
        for(int i = 0; i < tempTileArray.Length; i++)
        {
            if (tempTileArray[i].GetState() == Tile.TileState.BLOCK)
            {
                count++;
            }
            else
                count = 0;

        }

        if (count < 3)
        {
            foreach (Tile tile in playerHand)
            {
                if (tile.state == Tile.TileState.EMPTY)
                {
                    tile.state = DrawTile().GetState();
                }
            }
            return;
        }

        for(int i = 0; i < 2; i++)
        {
            if (playerHand[i].GetState() == Tile.TileState.EMPTY)
            {
                playerHand[i].state = DrawTile().GetState(); 
            }
        }

        // TODO SEARCH
    }

    private void SearchAndDraw(Tile.TileState state)
    {
        //TODO
    }

    private void CheckTopTile()
    {
        //TODO
    }

    private Tile DrawTile()
    {
        Tile tile = tileDeck[tileDeck.Count-1];
        tileDeck.RemoveAt(tileDeck.Count - 1);
        return tile;
    }

    public void DiscardTile(Tile tileToDiscard)
    {
        tileDeck.Add(tileToDiscard);


    }

    private void RecalculateBlockChance()
    {
        currBlockChance -= 5;
        if(blockCounter > 5)
        {
            currBlockChance = MAX_BLOCK_CHANCE;
            blockCounter = 0;
        }
    }

    public Tile[] GetPlayerHand()
    {
        return playerHand;
    }

    // Update is called once per frame
    void Update () {
        FillHand();
	}
}
