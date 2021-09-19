
using Chessy.Models.Board.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Chessy.Models.Board;
public class Board
{
    public Guid Id {  get; }
    public List<Piece> Pieces {  get; set;}
    public Color NowPlaying { get; set; }
    public List<Piece> Captured { get; set; }
    public (Cell Source, Cell Destination) LastMove {  get; set; }
    public bool Promotion { get; set; }
    public bool Started {  get; set; }
    public bool SecondPlayerConnected {  get; set; }
    public bool IsSecondPlayer {  get; set; }
    public bool Checked {  get; set; }
    public bool GameOver { get; private set; }
    public Color SecondPlayerColor {  get; set;}
    public int Timeframe { get; set; }
    public int WhiteTimeLeft { get; private set; }
    public int BlackTimeLeft { get; private set; }
    private System.Timers.Timer timer;

    /// <summary>
    /// EventHandler to update view with Title.
    /// </summary>
    public event Func<bool, Task> Notify;

    public Board(int timeFrame = 30)
    {
        Id = Guid.NewGuid();

        NowPlaying = Color.White;
        Captured = new List<Piece>();
        Pieces = new List<Piece>();

        Timeframe = timeFrame;
        WhiteTimeLeft = timeFrame;
        BlackTimeLeft = timeFrame;
    }

    public void ResetBoard()
    {
        Pieces.Clear();

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

        NowPlaying = Color.White;
        Captured = new List<Piece>();


        WhiteTimeLeft = Timeframe;
        BlackTimeLeft = Timeframe;

        GameOver = false;

        if (timer is not null)
        {
            timer.Stop();
            timer.Dispose();
            timer = null;
        }

        Update(true);
    }

    internal void Move(Cell selected, Cell target)
    {
        selected.LastMove = true;
        target.LastMove = true;
        Start();
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

    public void Start()
    {
        if (timer is not null)
            return;
        Started = true;
        timer = new System.Timers.Timer(1000);
        timer.AutoReset = true; 
        timer.Elapsed += timer_elapsed;
        timer.Start();
    }

    private void timer_elapsed(object? sender, ElapsedEventArgs e)
    {
        if (NowPlaying == Color.White)
        {
            WhiteTimeLeft--;
            if (WhiteTimeLeft == 0)
            {
                GameOver = true;
                timer.Stop();
            }
        }
        else
        {
            BlackTimeLeft--;
            if (BlackTimeLeft == 0)
            {
                GameOver = true;
                timer.Stop();
            }

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

    public async Task Update(bool Clear = false)
    {
        if (Notify != null)
        {
            await Notify.Invoke(Clear);
        }
    }

    public string Points(Color color)
    {
        int w = 0, b = 0;
        foreach(Piece p in Captured)
        {
            if (p.Color == Color.White)
                b += p.Value;
            else
                w += p.Value;
        }
        if(w > 900)
        {
            GameOver = true;
            return color == Color.White ? "Winner" : "Looser";
        }
        else if (b > 900)
        {
            GameOver = true;
            return color == Color.Black ? "Winner" : "Looser";
        }

        return "" + (color == Color.White ? w - b : b - w);
    }
}
