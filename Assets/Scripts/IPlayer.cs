using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer {
    void SetupPlayerControls(BoardManager boardManager, int playerID);
    bool MoveTile(PlaceScript place);
    bool MoveAttacker(PlaceScript place);
    bool DestroyTile(PlaceScript place);
}
