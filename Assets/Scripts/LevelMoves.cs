using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMoves : Level
{
    //规定游戏步数
    public int numMoves;
    public FruitPiece.FruitType[] fruits;
    //规定目标数量
    public int[] goals;
    private int movesUsed = 0;
    private bool hasWon = false;
    void Start()
    {
        type = LevelType.Moves;
        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(goals[0], goals[1]);
        hud.SetRemaining(numMoves);
        //Debug.Log("Number of moves:" + numMoves + "Target score:" + targetScore);
    }
    public override void OnMove()
    {
        movesUsed++;
        hud.SetRemaining(numMoves-movesUsed);
        if (numMoves - movesUsed == 0 && (goals[0] >0||goals[1] >0))
        {
            GameLose();
        }
    }
    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared(piece);
        StartCoroutine(ClearObstacle(piece));
    }
    public IEnumerator ClearObstacle(GamePiece piece)
    {
        if (hasWon) yield break;
        bool goalCompleted = false;
        for (int i = 0; i < fruits.Length; i++)
        {
            if (fruits[i] == piece.FruitComponent.Fruit)
            {
                if (goals[i] > 0)
                {
                    goals[i]--;
                    if (goals[i] <= 0)
                    {
                        goals[i] = 0;
                    }
                }
                hud.SetTarget(goals[0], goals[1]);
                if (goals[0] == 0 && goals[1] == 0 && !hasWon)
                {
                    goalCompleted= true;
                }
            }
        }
        if (goalCompleted)
        {
            hasWon = true;
            currentScore += 1000 * (numMoves - movesUsed-1);
            hud.SetScore(currentScore);
            GameWin();
        }
        yield return new WaitForSeconds(0.05f);
    }
}
