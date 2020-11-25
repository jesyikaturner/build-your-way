using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardJsonReader : MonoBehaviour
{

    // To be called in BoardManager. Taking in the json file from BoardManager and then
    // returns a filled array. should be called TileInfo[] array = SetupBoard(jsonFile);
    public TileInfo[] SetupBoard(TextAsset jsonFile)
    {
        BoardDataArray arrayList = JsonUtility.FromJson<BoardDataArray>(jsonFile.text);
        List<TileInfo> tempList = new List<TileInfo>();

        foreach(BoardData data in arrayList.tiles)
        {
            TileInfo tileClone = new TileInfo(data.state, data.team, data.x, data.y);
            tempList.Add(tileClone);
        }

        TileInfo[] array = tempList.ToArray();
        return array;
    }

#pragma warning disable 0649
    [System.Serializable]
    private class BoardDataArray
    {
        public BoardData[] tiles;
    }

    [System.Serializable]
    private class BoardData
    {
        public string state;
        public int team;
        public int x;
        public int y;
    }
}
