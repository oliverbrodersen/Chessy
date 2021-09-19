using Chessy.Models;
using Chessy.Models.Board;
using Chessy.Models.Board.Pieces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Chessy.Components;
public partial class BoardComponent
{
    [Parameter]
    public Board Board { get; set; }

    [Parameter]
    public Color PlayingColor { get; set; }

    [Parameter]
    public bool LocalTwoPlayer { get; set; }

    [Parameter]
    public bool Spectating { get; set; }

    public Cell[,] Grid {  get; set; }
    
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
        if (!Spectating && (PlayingColor == Board.NowPlaying || LocalTwoPlayer) && !Board.GameOver)
        {
            if (cell.Selected)
            {
                DeselectAll();
                return;
            }

            Cell selected = GetSelected();
            if (selected is not null && cell.Possible)
            {
                DeselectAll(true, false);
                Board.Move(selected, cell);
                DeselectAll(false, true);
                MarkNextLegalMoves(GetPiece(cell), true);
                return;
            }
            else if (!IsEmpty(cell) && GetPiece(cell).Color == Board.NowPlaying)
            {
                DeselectAll();
                //Player clicks own piece
                MarkNextLegalMoves(GetPiece(cell));
            }
            else
            {
                DeselectAll();
            }
            cell.Selected = true;
        }
    }

    private void DeselectAll(bool extended = false, bool castles = false)
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                Grid[i, j].Selected = false;
                Grid[i, j].Possible = false;

                if (extended)
                {
                    Grid[i, j].LastMove = false;
                }

                if (castles)
                    Grid[i, j].Castles = false;
            }
        }
    }

    private void MarkNextLegalMoves(Piece piece, bool CheckOnly = false)
    {
        if (piece.Color == PlayingColor || LocalTwoPlayer)
        {
            switch (piece.Type)
            {
                case PieceType.Pawn:
                    Piece rPiece = GetPiece(new Cell(piece.Position.Row + piece.MovementPattern[0][0], piece.Position.Col + 1));
                    Piece lPiece = GetPiece(new Cell(piece.Position.Row + piece.MovementPattern[0][0], piece.Position.Col - 1));
                    if(rPiece is not null && rPiece.Color != piece.Color)
                        MarkPossible(piece, piece.MovementPattern[0][0], 1, CheckOnly);
                    if(lPiece is not null && lPiece.Color != piece.Color)
                        MarkPossible(piece, piece.MovementPattern[0][0], -1, CheckOnly);

                    if(GetPiece(new Cell(piece.Position.Row + piece.MovementPattern[0][0], piece.Position.Col)) is null)
                    {
                        MarkPossible(piece, piece.MovementPattern[0][0], 0, CheckOnly); 
                        if (piece.FirstMove && GetPiece(new Cell(piece.Position.Row + ( piece.MovementPattern[0][0] * 2 ), piece.Position.Col)) is null)
                        {
                            MarkPossible(piece, piece.MovementPattern[0][0] * 2, 0, CheckOnly);
                        }
                    }
                    break;
                case PieceType.King:
                    foreach (var i in piece.MovementPattern)
                    {
                        MarkPossible(piece, i[0], i[1], CheckOnly);
                    }

                    if (MarkPossible(piece, 0, 1, CheckOnly) && piece.FirstMove)
                    {
                        Piece rRayPiece = MarkRay(piece, 0, 1, true);
                        if (rRayPiece.Type == PieceType.Rook && piece.FirstMove && piece.Color == rRayPiece.Color)
                        {
                            MarkPossible(piece, 0, 2, CheckOnly, true);
                        }
                    }
                    if (MarkPossible(piece, 0, -1, CheckOnly) && piece.FirstMove)
                    {
                        Piece lRayPiece = MarkRay(piece, 0, -1, true);
                        if (lRayPiece.Type == PieceType.Rook && piece.FirstMove && piece.Color == lRayPiece.Color)
                        {
                            lRayPiece.Position.Castles = true;
                            MarkPossible(piece, 0, -2, CheckOnly, true);
                        }
                    }
                    break;
                case PieceType.Knight:
                    foreach (var i in piece.MovementPattern)
                    {
                        MarkPossible(piece, i[0], i[1], CheckOnly);
                    }
                    break;
                case PieceType.Rook:
                case PieceType.Bishop:
                case PieceType.Queen:
                    foreach (var i in piece.MovementPattern)
                    {
                        MarkRay(piece, i[0], i[1], CheckOnly);
                    }
                    break;
            }
        }
    }

    private Piece MarkRay(Piece piece, int row, int col, bool onlyCheck = false)
    {
        for (int i = 1; i < 8; i++)
        {
            if(!MarkPossible(piece, row * i, col * i, onlyCheck))
                return GetPiece(new Cell(piece.Position.Row + (i * row), piece.Position.Col + (i * col)));
        }
        return null;
    }

    private bool MarkPossible(Piece piece, int row, int col, bool justCheck = false, bool castles = false)
    {
        if (piece.Position.Row + row < 8 && piece.Position.Row + row >= 0 &&
            piece.Position.Col + col < 8 && piece.Position.Col + col >= 0)
        {
            Piece hit = Board.GetPiece(piece.Position.Row + row, piece.Position.Col + col);
            if (hit is null || hit.Color != piece.Color)
            {
                if (hit is not null && hit is King)
                {
                    hit.Checked = true;
                    Board.Update();
                }

                if (justCheck)
                    return true;

                Grid[piece.Position.Row + row, piece.Position.Col + col].Possible = true;
                Grid[piece.Position.Row + row, piece.Position.Col + col].Castles = castles;
                return true;
            }
        }
        return false;
    }

    public void PromotePiece(PieceType newPiece)
    {
        Board.Promotion = false;
        Board.Pieces.Remove(GetPiece(Board.LastMove.Destination));
        switch (newPiece)
        {
            case PieceType.Queen:
                Board.Pieces.Add(new Queen(Board.NowPlaying, Board.LastMove.Destination, false));
                break;
            case PieceType.Knight:
                Board.Pieces.Add(new Knight(Board.NowPlaying, Board.LastMove.Destination, false));
                break;
            case PieceType.Rook:
                Board.Pieces.Add(new Rook(Board.NowPlaying, Board.LastMove.Destination, false));
                break;
            case PieceType.Bishop:
                Board.Pieces.Add(new Bishop(Board.NowPlaying, Board.LastMove.Destination, false));
                break;
        }
        Board.NextTurn();
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

    public async Task OnNotify(bool clear)
    {
        await InvokeAsync(() =>
        {
            if(clear)
                DeselectAll();
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
