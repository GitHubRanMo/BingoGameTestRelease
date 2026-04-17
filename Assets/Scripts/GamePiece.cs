using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

    public int score;

    private int x;
    private int y;
    public int X
    {
        get { return x; }
        set
        {
            if (IsMovable())
            {
                x = value;
            }
        }
    }
    public int Y
    {
        get { return y; }
        set
        {
            if (IsMovable())
            {
                y = value;
            }
        }
    }
    private GameGrid.PieceType type;
    public GameGrid.PieceType Type
    {
        get { return type; }
    }
    private GameGrid grid;
    public GameGrid GridRef
    {
        get { return grid; }
    }
    private MoveablePiece moveableComponent;
    public MoveablePiece MoveableComponent
    {
        get { return moveableComponent; }
    }
    private FruitPiece fruitComponent;
    public FruitPiece FruitComponent
    {
        get { return fruitComponent; }
    }
    private ClearablePiece clearableComponent;
    public ClearablePiece ClearableComponent
    {
        get { return clearableComponent; }
    }
    void Awake()
    {
        fruitComponent = GetComponent<FruitPiece>();
        moveableComponent = GetComponent<MoveablePiece>();
        clearableComponent = GetComponent<ClearablePiece>();
    }
    public void Init(int _x,int _y,GameGrid _grid,GameGrid.PieceType _type)
    {
        x = _x;
        y = _y;
        grid= _grid;
        type = _type;
    }
    void OnMouseEnter()
    {
        grid.EnterPiece(this);
    }
    void OnMouseDown()
    {
        grid.PressPiece(this);
    }
    void OnMouseUp()
    {
        grid.ReleasePiece();
    }
    public bool IsMovable()
    {
        return moveableComponent != null;
    }
    public bool IsFruit()
    {
        return fruitComponent != null;
    }
    public bool IsClearable()
    {
        return clearableComponent != null;
    }
}
