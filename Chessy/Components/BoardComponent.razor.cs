using Chessy.Models;
using Chessy.Models.Board;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Chessy.Components;
public partial class BoardComponent
{
    [Parameter]
    public Board Board { get; set; }

    [Parameter]
    public Color PlayingColor { get; set; }

    public Cell[,] Grid {  get; set; }

    public bool LocalTwoPlayer { get; set; }
    
    private string[] letters;
    protected override void OnInitialized()
    {
        letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        Board.Notify += OnNotify;
        LocalTwoPlayer = true;
        Grid = new Cell[8,8];
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Grid[i, j] = new Cell(i, j);
            }
        }
    }

    public string Icon(Cell cell)
    {
        Piece piece = GetPiece(cell);
        if (piece is not null)
            return piece.Color + "_" + piece.Type.ToString();
        return "empty";
    }

    public Piece GetPiece(Cell cell)
    {
        foreach(Piece piece in Board.Pieces)
        {
            if(piece.On(cell))
                return piece;
        }
        return null;
    }

    public async Task ClickCell(Cell cell, DragEventArgs dragEventArgs = null)
    {
        if (cell.Selected)
        {
            DeselectAll();
            return;
        }

        DeselectAll();

        Cell selected = GetSelected();
        if (selected is not null && cell.Possible)
        {
            //Move
        }
        else if (!IsEmpty(cell) && GetPiece(cell).Color == PlayingColor)
        {
            //Player clicks own piece
            MarkNextLegalMoves(GetPiece(cell));
        }
        cell.Selected = true;
    }

    private void DeselectAll()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Grid[i, j].Selected = false;
                Grid[i, j].Possible = false;
            }
        }
    }

    private void MarkNextLegalMoves(Piece piece)
    {
        if (piece.Color == PlayingColor)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    if(MarkPossible(piece, piece.MovementPattern[0][0], piece.MovementPattern[0][1]) && piece.FirstMove)
                        MarkPossible(piece, piece.MovementPattern[0][0] * 2, piece.MovementPattern[0][1] * 2);
                    break;
                case PieceType.King:
                case PieceType.Knight:
                    foreach (var i in piece.MovementPattern)
                    {
                        MarkPossible(piece, i[0], i[1]);
                    }
                    break;
                case PieceType.Rook:
                case PieceType.Bishop:
                case PieceType.Queen:
                    foreach (var i in piece.MovementPattern)
                    {
                        MarkRay(piece, i[0], i[1]);
                    }
                    break;
            }
        }
    }

    private void MarkRay(Piece piece, int row, int col)
    {
        for (int i = 1; i < 8; i++)
        {
            MarkPossible(piece, row * i, col * i);
            if (CheckCollision(piece, i * row, i * col))
                break;
        }
    }

    private bool MarkPossible(Piece piece, int row, int col)
    {
        if (piece.Position.Row + row < 8 && piece.Position.Row + row >= 0 &&
            piece.Position.Col + col < 8 && piece.Position.Col + col >= 0)
        {
            Piece hit = Board.GetPiece(piece.Position.Row + row, piece.Position.Col + col);
            if (hit is null || hit.Color != PlayingColor)
            {
                Grid[piece.Position.Row + row, piece.Position.Col + col].Possible = true;
                return true;
            }
        }
        return false;
    }


    private bool CheckCollision(Piece piece, int row, int col)
    {
        if (piece.Position.Row + row < 8 && piece.Position.Row + row >= 0 && piece.Position.Col + col < 8 && piece.Position.Col + col >= 0)
        {
            return !IsEmpty(Grid[piece.Position.Row + row, piece.Position.Col + col]);
        }

        return false;
    }

    public bool IsEmpty(Cell cell)
    {
        foreach(Piece piece in Board.Pieces)
        {
            if (piece.Position.Row == cell.Row && piece.Position.Col == cell.Col)
                return false;
        }
        return true;
    }

    public async Task OnNotify(Board board)
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    public Cell GetSelected()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                if (Grid[i,j].Selected)
                    return Grid[i,j];
            }
        }
        return null;
    }

    private Color OpponentColor()
    {
        return PlayingColor == Color.White ? Color.Black : Color.White;
    }
}
