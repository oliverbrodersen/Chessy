
namespace Chessy.Models.Board;
public class Board
{
    public Guid Id {  get; }
    public int Size { get; set; }
    public Cell[,] Grid { get; set; }

    public Color NowPlaying { get; set; }

    public List<Piece> Captured { get; set; }

    public (Cell Source, Cell Destination) LastMove {  get; set; }

    public bool Promotion { get; set; }

    public bool Started {  get; set; }

    public bool SecondPlayerConnected {  get; set; }

    public Color SecondPlayerColor {  get; set;}

    /// <summary>
    /// EventHandler to update view with Title.
    /// </summary>
    public event Func<Board, Task> Notify;
    public Board(int size)
    {
        Id = Guid.NewGuid();

        Size = size;
        NowPlaying = Color.White;
        Captured = new List<Piece>();
        Grid = new Cell[Size, Size];
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Grid[i, j] = new Cell(i, j);
            }
        }
    }

    public void ResetBoard()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Grid[i, j] = new Cell(i, j);
            }
        }

        Grid[0, 0].SetPiece(new Piece(PieceType.Rook,   Color.White));
        Grid[0, 1].SetPiece(new Piece(PieceType.Knight, Color.White));
        Grid[0, 2].SetPiece(new Piece(PieceType.Bishop, Color.White));
        Grid[0, 3].SetPiece(new Piece(PieceType.King,   Color.White));
        Grid[0, 4].SetPiece(new Piece(PieceType.Queen,  Color.White));
        Grid[0, 5].SetPiece(new Piece(PieceType.Bishop, Color.White));
        Grid[0, 6].SetPiece(new Piece(PieceType.Knight, Color.White));
        Grid[0, 7].SetPiece(new Piece(PieceType.Rook,   Color.White));

        Grid[7, 0].SetPiece(new Piece(PieceType.Rook,   Color.Black));
        Grid[7, 1].SetPiece(new Piece(PieceType.Knight, Color.Black));
        Grid[7, 2].SetPiece(new Piece(PieceType.Bishop, Color.Black));
        Grid[7, 3].SetPiece(new Piece(PieceType.King,   Color.Black));
        Grid[7, 4].SetPiece(new Piece(PieceType.Queen,  Color.Black));
        Grid[7, 5].SetPiece(new Piece(PieceType.Bishop, Color.Black));
        Grid[7, 6].SetPiece(new Piece(PieceType.Knight, Color.Black));
        Grid[7, 7].SetPiece(new Piece(PieceType.Rook,   Color.Black));

        for (int i = 0; i < Size; i++)
        {
            Grid[1, i].SetPiece(new Piece(PieceType.Pawn, Color.White));
            Grid[6, i].SetPiece(new Piece(PieceType.Pawn, Color.Black));
        }
    }

    public void Print()
    {
        for (int i = Size-1; i >= 0; i--)
        {
            for (int j = 0; j < Size - 1; j++)
            {
                if(j > 0)
                    Console.Write("|");
                if(j == Size-2)
                    Console.WriteLine(Grid[i,j]);
                else
                    Console.Write(Grid[i,j]); 
            }
            if(i>Size-1)
                Console.WriteLine("---------------");
        }
    }

    public void MarkNextLegalMoves(Cell CurrentCell)
    {
        if (!CurrentCell.Empty && CurrentCell.Piece.Color == NowPlaying)
        {
            switch (CurrentCell.Piece.Type)
            {
                case PieceType.Knight:
                    SetPossible(CurrentCell, 2, 1);
                    SetPossible(CurrentCell, 2, -1);
                    SetPossible(CurrentCell, -2, 1);
                    SetPossible(CurrentCell, -2, -1);
                    SetPossible(CurrentCell, 1, 2);
                    SetPossible(CurrentCell, 1, -2);
                    SetPossible(CurrentCell, -1, 2);
                    SetPossible(CurrentCell, -1, -2);
                    break;
                case PieceType.Rook:
                    Ray(CurrentCell, 1, 0);
                    Ray(CurrentCell, -1, 0);
                    Ray(CurrentCell, 0, 1);
                    Ray(CurrentCell, 0, -1);
                    break;
                case PieceType.Bishop:
                    Ray(CurrentCell, 1, 1);
                    Ray(CurrentCell, -1, 1);
                    Ray(CurrentCell, 1, -1);
                    Ray(CurrentCell, -1, -1);
                    break;
                case PieceType.Queen:
                    Ray(CurrentCell, 1, 1);
                    Ray(CurrentCell, -1, 1);
                    Ray(CurrentCell, 1, -1);
                    Ray(CurrentCell, -1, -1);
                    Ray(CurrentCell, 1, 0);
                    Ray(CurrentCell, -1, 0);
                    Ray(CurrentCell, 0, 1);
                    Ray(CurrentCell, 0, -1);
                    break;
                case PieceType.King:
                    SetPossible(CurrentCell,  1,  1);
                    SetPossible(CurrentCell,  -1,  -1);
                    SetPossible(CurrentCell,  -1,  1);
                    SetPossible(CurrentCell,  1,  -1);
                    SetPossible(CurrentCell,  1, 0);
                    SetPossible(CurrentCell,  -1, 0);
                    //Checks sides as well as castles
                    if(SetPossible(CurrentCell, 0, 1))
                    {
                        if (CurrentCell.Piece.FirstMove)
                        {
                            if(SetPossible(CurrentCell, 0, 2, true))
                            {
                                if (SetPossible(CurrentCell, 0, 3, true))
                                {
                                    if (Grid[CurrentCell.Row, CurrentCell.Col + 4].Piece.Type == PieceType.Rook &&
                                        Grid[CurrentCell.Row, CurrentCell.Col + 4].Piece.FirstMove)
                                    {
                                        SetPossible(CurrentCell, 0, 2, false, true);
                                    }
                                }
                            }
                        }
                    }
                    if (SetPossible(CurrentCell, 0, -1))
                    {
                        if (CurrentCell.Piece.FirstMove)
                        {
                            if (SetPossible(CurrentCell, 0, -2, true))
                            {
                                if (Grid[CurrentCell.Row, CurrentCell.Col - 3].Piece.Type == PieceType.Rook &&
                                        Grid[CurrentCell.Row, CurrentCell.Col - 3].Piece.FirstMove)
                                {
                                    SetPossible(CurrentCell, 0, -2, false, true);
                                }
                            }
                        }
                    }
                    break;
                case PieceType.Pawn:
                    int fact = CurrentCell.Piece.Color == Color.White ? 1 : -1;
                    if (CurrentCell.Row + fact < Size && CurrentCell.Row + fact >= 0)
                    {
                        if (Grid[CurrentCell.Row + fact, CurrentCell.Col].Empty)
                        {
                            if (CurrentCell.Piece.FirstMove && Grid[CurrentCell.Row + (2*fact), CurrentCell.Col].Empty)
                                SetPossible(CurrentCell, 2 * fact, 0);

                            SetPossible(CurrentCell, fact, 0);
                        }

                        //Check for diagonal captures
                        if (CurrentCell.Col - fact < Size && CurrentCell.Col - fact >= 0)
                        {
                            if (!Grid[CurrentCell.Row + fact, CurrentCell.Col - fact].Empty)
                                SetPossible(CurrentCell, fact, -fact);
                        }
                        if (CurrentCell.Col + fact < Size && CurrentCell.Col + fact >= 0)
                        {
                            if (!Grid[CurrentCell.Row + fact, CurrentCell.Col + fact].Empty)
                                SetPossible(CurrentCell, fact, fact);
                        }
                    }
                    break;
            }
        }
    }

    public Cell GetSelected()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Grid[i, j].Selected)
                    return Grid[i, j];
            }
        }
        return null;
    }

    public async Task Move(Cell Source, Cell Destination)
    {
        Started = true;

        if(LastMove.Destination != null)
        {
            LastMove.Destination.LastMove = false;
            LastMove.Source.LastMove = false;
        }

        Destination.LastMove = true;
        Source.LastMove = true;
        LastMove = (Source, Destination);

        if (!Destination.Empty)
        {
            Captured.Add(Destination.Piece);
            Captured.Sort((x, y) => x.Type.CompareTo(y.Type));
        }
        Destination.SetPiece(Source.Piece);
        Destination.Piece.FirstMove = false;
        Source.RemovePiece();
        if (Destination.Piece.Type == PieceType.Pawn &&
            (Destination.Row == 0 || Destination.Row == Size - 1))
        {
            Promotion = true;
        }
        else if (Destination.Castles)
        {
            if (Destination.Col == 1)
            {
                await Move(Grid[Destination.Row, 0], Grid[Destination.Row, 2]);
                Console.WriteLine("o-o");
            }
            else if(Destination.Col == 5)
            {
                await Move(Grid[Destination.Row, 7], Grid[Destination.Row, 4]);
                Console.WriteLine("o-o-o");
            }
        }
        else
        {
            NowPlaying = NowPlaying == Color.White ? Color.Black : Color.White;
        }
        DeselectAll();
        await Update();
    }
    public async Task Update()
    {
        if (Notify != null)
        {
            await Notify.Invoke(this);
        }
    }
    public void DeselectAll()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                Grid[i, j].Selected = false;
                Grid[i, j].Possible = false;
                Grid[i, j].Castles  = false;
            }
        }
    }
    public int Points(Color color)
    {
        int w = 0, b = 0;
        foreach(Piece p in Captured)
        {
            if (p.Color == Color.White)
                b += p.Points();
            else
                w += p.Points();
        }
        return color == Color.White ? w - b : b - w;
    }

    public override string? ToString()
    {
        return base.ToString();
    }
    private bool CheckCollision(Cell CurrentCell, int row, int col)
    {
        if (CurrentCell.Row + row < Size && CurrentCell.Row + row >= 0 && CurrentCell.Col + col < Size && CurrentCell.Col + col >= 0)
        {
            return !Grid[CurrentCell.Row + row, CurrentCell.Col + col].Empty;
        }
        return false;
    }

    private bool SetPossible(Cell CurrentCell, int row, int col, bool checkOnly = false, bool castles = false)
    {
        if(CurrentCell.Row + row < Size && CurrentCell.Row + row >= 0 && CurrentCell.Col + col < Size && CurrentCell.Col + col >= 0)
        {
            if (!Grid[CurrentCell.Row + row, CurrentCell.Col + col].Empty && CurrentCell.Piece.Color == Grid[CurrentCell.Row + row, CurrentCell.Col + col].Piece.Color)
                return false;
            if(!checkOnly)
                Grid[CurrentCell.Row + row, CurrentCell.Col + col].Possible = true;
            if(castles)
                Grid[CurrentCell.Row + row, CurrentCell.Col + col].Castles = true;
            return true;
        }
        return false;
    }

    private void Ray(Cell CurrentCell, int row, int col)
    {
        for (int i = 1; i < Size; i++)
        {
            SetPossible(CurrentCell, i * row, i * col);
            if (CheckCollision(CurrentCell, i * row, i * col))
                break;

        }
    }
}
