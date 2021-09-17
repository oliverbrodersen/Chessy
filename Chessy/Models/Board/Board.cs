
using Chessy.Models.Board.Pieces;

namespace Chessy.Models.Board;
public class Board
{
    private object board;
    public Guid Id {  get; }
    public List<Piece> Pieces {  get; set;}
    public Color NowPlaying { get; set; }
    public List<Piece> Captured { get; set; }
    public (Cell Source, Cell Destination) LastMove {  get; set; }
    public bool Promotion { get; set; }
    public bool Started {  get; set; }
    public bool SecondPlayerConnected {  get; set; }
    public bool Checked {  get; set; }
    public Color SecondPlayerColor {  get; set;}

    /// <summary>
    /// EventHandler to update view with Title.
    /// </summary>
    public event Func<Board, Task> Notify;

    public Board()
    {
        Id = Guid.NewGuid();

        NowPlaying = Color.White;
        Captured = new List<Piece>();
        Pieces = new List<Piece>();
    }

    public void ResetBoard()
    {
        Pieces.Add(new Rook     (Color.White, new Cell(0, 0)));
        Pieces.Add(new Knight   (Color.White, new Cell(0, 1)));
        Pieces.Add(new Bishop   (Color.White, new Cell(0, 2)));
        Pieces.Add(new King     (Color.White, new Cell(0, 3)));
        Pieces.Add(new Queen    (Color.White, new Cell(0, 4)));
        Pieces.Add(new Bishop   (Color.White, new Cell(0, 5)));
        Pieces.Add(new Knight   (Color.White, new Cell(0, 6)));
        Pieces.Add(new Rook     (Color.White, new Cell(0, 7)));

        Pieces.Add(new Rook     (Color.Black, new Cell(7, 0)));
        Pieces.Add(new Knight   (Color.Black, new Cell(7, 1)));
        Pieces.Add(new Bishop   (Color.Black, new Cell(7, 2)));
        Pieces.Add(new King     (Color.Black, new Cell(7, 3)));
        Pieces.Add(new Queen    (Color.Black, new Cell(7, 4)));
        Pieces.Add(new Bishop   (Color.Black, new Cell(7, 5)));
        Pieces.Add(new Knight   (Color.Black, new Cell(7, 6)));
        Pieces.Add(new Rook     (Color.Black, new Cell(7, 7)));

        for (int i = 0; i < 8; i++)
        {
            Pieces.Add(new Pawn(Color.White, new Cell(1, i)));
            Pieces.Add(new Pawn(Color.Black, new Cell(6, i)));
        }
    }

    internal void Move(Cell selected, Cell target)
    {
        selected.LastMove = true;
        target.LastMove = true;
        LastMove = (selected.Copy(), target.Copy());

        Piece piece = GetPiece(selected.Row, selected.Col);
        Piece targetPiece = GetPiece(target.Row, target.Col);
        if (targetPiece is not null)
            Capture(targetPiece);

        piece.Move(target);

        if (piece is Pawn && (piece.Position.Row == 0 || piece.Position.Row == 7))
            Promotion = true;
        else if(!target.Castles)
            NextTurn();
        else
        {
            if (target.Col == 1)
                Move(new Cell(target.Row, 0), new Cell(target.Row, 2));
            if (target.Col == 5)
                Move(new Cell(target.Row, 7), new Cell(target.Row, 4));
        }
        Update();
    }

    public void NextTurn()
    {
        NowPlaying = NowPlaying == Color.White ? Color.Black : Color.White;
    }

    private void Capture(Piece targetPiece)
    {
        targetPiece.Position = null;
        Pieces.Remove(targetPiece);
        Captured.Add(targetPiece);
    }

    public Piece GetPiece(int row, int col)
    {
        foreach(Piece piece in Pieces)
        {
            if(piece.Position.Row == row &&
                piece.Position.Col == col)
                return piece;
        }
        return null;
    }
    public Piece GetSelected()
    {
        return Pieces.Where(p => p.Selected).FirstOrDefault();
    }

    public async Task Update()
    {
        if (Notify != null)
        {
            await Notify.Invoke(this);
        }
    }

    public int Points(Color color)
    {
        int w = 0, b = 0;
        foreach(Piece p in Captured)
        {
            if (p.Color == Color.White)
                b += p.Value;
            else
                w += p.Value;
        }
        return color == Color.White ? w - b : b - w;
    }
}
