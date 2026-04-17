using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        NONORMAL,
        RAINBOW,
        COUNT
    };
    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    [System.Serializable]
    public struct PiecePosition
    {
        public PieceType type;
        public int x;
        public int y;
    };

    public int xDim;
    public int yDim;
    public float fillTime;

    public Level level;

    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;

    public PiecePosition[] initialPieces;
    private Dictionary<PieceType,GameObject> piecePrefabDict;
    private GamePiece[,] pieces;

    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    private bool gameOver = false;
    void Awake()
    {
        piecePrefabDict=new Dictionary<PieceType, GameObject>();
        for(int i=0;i<piecePrefabs.Length;i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x,y), Quaternion.identity);
                background.transform.parent= transform;
            }
        }
        pieces=new GamePiece[xDim,yDim];
        for (int i = 0;i < initialPieces.Length; i++){
            if (initialPieces[i].x >= 0 && initialPieces[i].x<xDim
                && initialPieces[i].y >= 0 && initialPieces[i].y < yDim)
            {
                SpawnNewPiece(initialPieces[i].x, initialPieces[i].y, initialPieces[i].type);
            }
        }
        for (int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if (pieces[x, y] == null)
                {
                    SpawnNewPiece(x, y, PieceType.EMPTY);
                } 
            }
        }
        StartCoroutine(Fill());
    }
    public Vector2 GetWorldPosition(int x,int y)
    {
        return new Vector2(transform.position.x-xDim/2.0f+x + 0.5f, transform.position.y+yDim/2.0f-y - 0.5f);//- transform.parent.position.y  -0.5f
    }
   
    public IEnumerator Fill()
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches();
        }
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        // y = 0 表示在顶部，我们忽略最后一行，因为这一行无法再向下移动
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++) //不规则处有变动
            {
                int x = loopX;
                if (inverse)//障碍物则斜插直下
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if(pieceBelow.Type==PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MoveableComponent.Move(x, y+1,fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y,PieceType.EMPTY);  
                        movedPiece = true;
                    }
                    else
                    {
                        //diag:斜率
                        for(int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim)  //不规则变化改动
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for(int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }
                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MoveableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Debug.Log("fill");
            }
        }
        

        for (int x = 0; x < xDim; x++)  //不规则变化改动
        {
            GamePiece pieceBelow = pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent= transform;

                pieces[x, 0]=newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x,-1,this,PieceType.NORMAL);
                pieces[x, 0].MoveableComponent.Move(x, 0,fillTime);
                pieces[x, 0].FruitComponent.SetFruit((FruitPiece.FruitType)Random.Range(0, pieces[x, 0].FruitComponent.NumFruits));
                movedPiece = true;
            }
        }
        return movedPiece;
    }
    //实例化GamePiece对象
    private GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }
    //拖动互换元素是否相邻
    public static bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1)
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }
    //执行互换元素方法
    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (gameOver)
        {
            return;
        }
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            if(GetMatch(piece1,piece2.X,piece2.Y)!=null||GetMatch(piece2,piece1.X,piece1.Y)!=null
                ||piece1.Type==PieceType.RAINBOW||piece2.Type==PieceType.RAINBOW)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MoveableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MoveableComponent.Move(piece1X, piece1Y, fillTime);

                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsFruit())
                {
                    ClearFruitPiece clearFruit = piece1.GetComponent<ClearFruitPiece>();
                    if (clearFruit)
                    {
                        clearFruit.Fruit = piece2.FruitComponent.Fruit;
                    }
                    ClearPiece(piece1.X, piece1.Y);
                }
                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsFruit())
                {
                    ClearFruitPiece clearFruit = piece2.GetComponent<ClearFruitPiece>();
                    if (clearFruit)
                    {
                        clearFruit.Fruit = piece1.FruitComponent.Fruit;
                    }
                    ClearPiece(piece2.X, piece2.Y);
                }
                ClearAllValidMatches();

                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                }
                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }
                pressedPiece = null;
                enteredPiece= null;
                StartCoroutine(Fill());
                level.OnMove();
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y]=piece2;
            }
            
        }
    }
    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
        //Debug.Log(piece.X);
    }
    //鼠标悬停执行方法
    public void EnterPiece(GamePiece piece)
    {
        enteredPiece= piece;
    }
    public void ReleasePiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }
    public List<GamePiece> GetMatch(GamePiece piece,int newX,int newY)
    {
        if (piece.IsFruit())
        {
            //相同元素列表
            FruitPiece.FruitType fruit = piece.FruitComponent.Fruit;
            List<GamePiece> horizontalPieces=new List<GamePiece>();
            List<GamePiece> verticalPieces=new List<GamePiece>();
            List<GamePiece> matchingPieces=new List<GamePiece>();

            horizontalPieces.Add(piece);
            //dir：方向 0左 1右
            for(int dir = 0; dir <= 1; dir++)
            {
                for(int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0)
                    {
                        x = newX - xOffset;
                    }
                    else
                    {
                        x = newX + xOffset;
                    }
                    if (x < 0 || x >= xDim) break;
                    if (pieces[x, newY].IsFruit() && pieces[x,newY].FruitComponent.Fruit==fruit)
                    {
                        horizontalPieces.Add(pieces[x,newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (horizontalPieces.Count >= 3)
            {
                for(int i=0;i<horizontalPieces.Count;i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }
            if(horizontalPieces.Count>=3)
            {
                for(int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0)
                            {
                                y = newY - yOffset;
                            }
                            else
                            {
                                y = newY + yOffset;
                            }
                            if (y < 0 || y >= yDim) break;
                            if (pieces[horizontalPieces[i].X, y].IsFruit() && pieces[horizontalPieces[i].X, y].FruitComponent.Fruit == fruit)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for(int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if(matchingPieces.Count>=3)
            {
                return matchingPieces;
            }
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);
            //dir：方向 0上 1下
            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < xDim; yOffset++) //xDim
                {
                    int y;
                    if (dir == 0)
                    {
                        y = newY - yOffset;
                    }
                    else
                    {
                        y = newY + yOffset;
                    }
                    if (y < 0 || y >= yDim) break;
                    if (pieces[newX, y].IsFruit() && pieces[newX, y].FruitComponent.Fruit == fruit)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < yDim; xOffset++)//yDim
                        {
                            int x;
                            if (dir == 0)
                            {
                                x = newX - xOffset;
                            }
                            else
                            {
                                x = newX + xOffset;
                            }
                            if (x < 0 || x >= xDim) break;
                            if (pieces[x, verticalPieces[i].Y].IsFruit() && pieces[x, verticalPieces[i].Y].FruitComponent.Fruit == fruit)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        return null;
    }
    //所有物体配对
    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;
        for(int y = 0; y < yDim; y++)
        {
            for(int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;
                        if (match.Count == 4)
                        {
                            //创建四消特殊消除，非移动产生四消，随机指定物体成为特殊物体，否则移动同色为特殊物体
                            if (pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            }
                            else if(pressedPiece.Y==enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                            else
                            {
                                specialPieceType= PieceType.COLUMN_CLEAR;
                            }
                        }else if(match.Count>=5){
                            specialPieceType = PieceType.RAINBOW;
                        }
                        for(int i=0;i<match.Count;i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true; 
                                //确定移动的同色物体的X，Y值
                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }
                        if (specialPieceType != PieceType.COUNT)
                        {
                            //销毁原物体，并生成特殊物体
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);
                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR)
                                && newPiece.IsFruit() && match[0].IsFruit())
                            {
                                newPiece.FruitComponent.SetFruit(match[0].FruitComponent.Fruit);
                            }else if (specialPieceType == PieceType.RAINBOW && newPiece.IsFruit())
                            {
                                newPiece.FruitComponent.SetFruit(FruitPiece.FruitType.Any);
                            }
                        }
                    }
                    
                }
            }
        }
        return needsRefill;
    }
    //清除游戏物体代码
    public bool ClearPiece(int x,int y)
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            pieces[x,y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);
            ClearObstacles(x, y);
            return true;
        }
        return false;
    }
    //清除障碍物逻辑
    public void ClearObstacles(int x,int y)
    {
        for(int adjacentX=x-1;adjacentX<=x+1;adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }   
        }
        for(int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x,adjacentY].Type==PieceType.BUBBLE&&pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }
    public void ClearRow(int row)
    {
        for(int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }
    public void ClearColumn(int column)
    {
        for(int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }
    public void ClearFruit(FruitPiece.FruitType fruit)
    {
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].IsFruit() && (pieces[x, y].FruitComponent.Fruit == fruit)
                    || (fruit == FruitPiece.FruitType.Any))
                {
                    ClearPiece(x, y);
                }
            }
        }
        
    }
    public void GameOver()
    {
        gameOver = true;
    }

    public List<GamePiece> GetPiecesOfType(PieceType type)
    {
        List<GamePiece> piecesOfType=new List<GamePiece>();
        for(int x = 0; x < xDim; x++)
        {
            for(int y = 0; y < yDim; y++)
            {
                if (pieces[x, y].Type == type)
                {
                    piecesOfType.Add(pieces[x, y]);
                }
            }
            
        }
        return piecesOfType;
    }
}
