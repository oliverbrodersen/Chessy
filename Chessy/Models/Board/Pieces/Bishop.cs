
using System.Collections.Generic;

namespace Chessy.Models.Board.Pieces;
public class Bishop : Piece
{
    public Bishop(Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.Bishop;
        Value = 5;
        Letter = 'B';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { 1, 1 });
        MovementPattern.Add(new int[] { -1, 1 });
        MovementPattern.Add(new int[] { -1, -1 });
        MovementPattern.Add(new int[] { 1, -1 });
    }
}
