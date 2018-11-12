using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer {
    void SetupPlayerControls(SoundManager soundManager, BoardManager boardManager, int playerID);
    bool MoveTile(Tile place);
    bool MoveAttacker(Tile place);
    bool DestroyTile(Tile place);
}
