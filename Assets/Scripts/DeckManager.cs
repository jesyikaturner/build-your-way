using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    // public Tile tilePrefab;

    // private List<Tile> _tileDeck;
    // private readonly uint _deckSize = 50;
    // private readonly float _tileOffset = -20f;

    // private void Start()
    // {
    //     if(tilePrefab)
    //     {
    //         PopulateQueue();
    //     }else
    //     {
    //         Debug.LogError("tilePrefab hasn't been assigned a value");
    //         Debug.Break();
    //     }
    // }

    // private void PopulateQueue()
    // {
    //     _tileDeck = new List<Tile>();

    //     for(int i = 0; i < _deckSize; i++)
    //     {
    //         Tile tileClone = Instantiate(tilePrefab, new Vector3(_tileOffset, 0), Quaternion.identity);
            
    //         if (i < (_deckSize / 3))
    //             tileClone.GetTileInfo().SetState(TileInfo.TileState.BLOCK);
    //         else
    //             tileClone.GetTileInfo().SetState(TileInfo.TileState.WALK);

    //         _tileDeck.Add(tileClone);
    //     }

    //     ShuffleDeck();
    // }

    // public List<Tile> GetDeck()
    // {
    //     return _tileDeck;
    // }

    // public Tile DrawCard()
    // {
    //     Tile firstTile = _tileDeck[_tileDeck.Count - 1];
    //     _tileDeck.RemoveAt(_tileDeck.Count - 1);

    //     return firstTile;
    // }

    // public void DiscardTile(Tile discardedTile)
    // {
    //     _tileDeck.Add(discardedTile);
    // }

    // public void ShuffleDeck()
    // {
    //     _tileDeck.Shuffle();
    // }
}
