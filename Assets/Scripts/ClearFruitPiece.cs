using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearFruitPiece : ClearablePiece
{
    private FruitPiece.FruitType fruit;
    public FruitPiece.FruitType Fruit
    {
        get { return fruit; }
        set { fruit = value; }
    }
    public override void Clear()
    {
        base.Clear();
        //Debug.Log(Fruit);
        piece.GridRef.ClearFruit(fruit);
    }
}
