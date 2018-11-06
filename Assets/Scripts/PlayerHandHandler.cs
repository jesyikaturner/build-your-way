using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandHandler : MonoBehaviour {

    private const int PLAYER_HAND_SIZE = 3;
    private const int MAX_BLOCK_CHANCE = 30;

    public PlaceScript[] playerHand;
    public int currBlockChance;
    public int blockCounter = 0;

    public void PopulatePlayerHand(int[,] coord, PlaceScript placement, int offset, float spacing, string name, int playerID)
    {
        playerHand = new PlaceScript[PLAYER_HAND_SIZE];
        currBlockChance = MAX_BLOCK_CHANCE;

        if (coord.GetLength(0) > PLAYER_HAND_SIZE)
        {
            Debug.LogError("coord length entered for player hand is greather than hand size");
            return;
        }

        for (int i = 0; i < coord.GetLength(0); i++)
        {
            PlaceScript handTile = Instantiate(placement, new Vector3(coord[i, 0] + offset * spacing, 0, coord[i, 1] * spacing), Quaternion.identity);
            handTile.SetState("EMPTY");
            handTile.name = name;
            handTile.playerHand = playerID;
            playerHand[i] = handTile;
        }
    }

    public void FillHand()
    {
        foreach (PlaceScript tile in playerHand)
        {
            if (tile.state == PlaceScript.PlaceState.EMPTY)
            {
                if (Random.Range(0, 100) <= currBlockChance)
                {
                    tile.SetState("BLOCK");
                    blockCounter++;
                    RecalculateBlockChance();
                }

                else
                {
                    tile.SetState("WALK");
                    blockCounter += 2;
                    RecalculateBlockChance();
                }

            }
        }
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

    public PlaceScript[] GetPlayerHand()
    {
        return playerHand;
    }

    // Update is called once per frame
    void Update () {
        FillHand();
	}
}
