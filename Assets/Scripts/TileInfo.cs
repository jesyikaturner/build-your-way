using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo
{
    private readonly string state;
    private readonly int team;
    private readonly int xPos, zPos;
    
    public TileInfo(string state, int team, int z, int x)
    {
        this.state = state;
        this.team = team;
        xPos = x;
        zPos = z;
    }

    public string GetState()
    {
        return state;
    }

    public int GetTeam()
    {
        return team;
    }

    public int GetXPos()
    {
        return xPos;
    }

    public int GetZPos()
    {
        return zPos;
    }
}
