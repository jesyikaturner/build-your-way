using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController {
    void SetupControls(SoundManager soundManager, BoardManager boardManager, int playerID);
    void ToggleActive();
    bool MoveTile(Tile place);
    bool MoveAttacker(Tile place);
    bool DestroyTile(Tile place);
}
