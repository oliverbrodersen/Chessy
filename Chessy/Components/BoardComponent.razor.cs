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

    public bool LocalTwoPlayer { get; set; }
    
    private string[] letters;
    protected override void OnInitialized()
    {
        letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        Board.Notify += OnNotify;
    }

    public async Task ClickCell(int row, int col, DragEventArgs dragEventArgs = null)
    {
        if ((PlayingColor == Board.NowPlaying || LocalTwoPlayer) && 
            !Board.Promotion)
        {
            if (Board.Grid[row, col].Selected)
            {
                Board.DeselectAll();
            }
            else if (Board.GetSelected() is not null && Board.Grid[row, col].Possible)
            {
                //Move
                Cell SourceCell = Board.GetSelected();
                Cell DestinationCell = Board.Grid[row, col];

                await Board.Move(SourceCell, DestinationCell);
            }
            else
            {
                Board.DeselectAll();
                Board.Grid[row, col].Selected = true;
                Board.MarkNextLegalMoves(Board.Grid[row, col]);
            }
        }
    }
    public async Task OnNotify(Board board)
    {
        await InvokeAsync(() =>
        {
            StateHasChanged();
        });
    }

    private async Task PromotePiece(Piece piece)
    {
        Board.LastMove.Destination.SetPiece(piece);
        Board.NowPlaying = Board.NowPlaying == Color.White ? Color.Black : Color.White;
        Board.Promotion = false;
        await Board.Update();
    }

    private Color OpponentColor()
    {
        return PlayingColor == Color.White ? Color.Black : Color.White;
    }
}
