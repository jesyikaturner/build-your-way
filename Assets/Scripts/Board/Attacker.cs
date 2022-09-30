using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacker : MonoBehaviour {

    [SerializeField]
    private List<Tile> moveHistory;

    private SpriteRenderer sprite;
    public Sprite player1_attacker, player2_attacker;
    public Sprite s_player1_attacker, s_player2_attacker;

    private const int MAX_MOVE_HISTORY = 3;
    public int team = 0;
    private int attackerType = 0;

    // Use this for initialization
    private void Awake()
    {
        moveHistory = new List<Tile>();
    }

    void Start() {
        sprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        attackerType = team;
        
    }

    // Update is called once per frame
    void Update() {
        SetSprite();
    }

    void SetSprite()
    {
        switch (attackerType)
        {
            case -2:
                sprite.sprite = s_player2_attacker;
                break;
            case -1:
                sprite.sprite = s_player1_attacker;
                break;
            case 1:
                sprite.sprite = player1_attacker;
                break;
            case 2:
                sprite.sprite = player2_attacker;
                break;
            default:
                Debug.LogError("Sprite cannot be set!");
                break;
        }
    }

    public void ToggleSelect()
    {
        attackerType *= -1;
    }

    public int Team{
        get
        {
            return team;
        }
        set
        {
            team = value;
        }
    }

    /*
    * Keeps the movehistory to a set size. If it exceeds the limit, then it clears
    * the list and adds the new entry.
    */
    public void UpdateHistory(Tile selectedMove)
    {
        if (moveHistory.Count < MAX_MOVE_HISTORY)
        {
            moveHistory.Add(selectedMove);
        }
        else
        {
            moveHistory.Clear();
            moveHistory.Add(selectedMove);
        }
    }

    /*
    * Checks if a move in the possiblemoves list is already in the movehistory list.
    */
    public bool CheckHistory(List<Tile> possibleMoves)
    {
        foreach (Tile cell in possibleMoves)
        {
            if (moveHistory.Contains(cell))
                return true;
        }
        return false;
    }

    public bool CheckHistory(Tile possibleMove)
    {
        if (moveHistory.Contains(possibleMove))
            return true;
        return false;
    }

    public void ClearHistory(Tile currentTile)
    {
        moveHistory.Clear();
        moveHistory.Add(currentTile);
    }
}
