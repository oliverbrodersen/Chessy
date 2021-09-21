using System.Collections.Generic;

namespace Chessy.Models.Board.Pieces;
public class King : Piece
{
    public King(Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.King;
        Value = 1000;
        Letter = 'K';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { 1, 0 });
        MovementPattern.Add(new int[] { -1, 0 });
        MovementPattern.Add(new int[] { 1, 1 });
        MovementPattern.Add(new int[] { -1, 1 });
        MovementPattern.Add(new int[] { -1, -1 });
        MovementPattern.Add(new int[] { 1, -1 });
    }
}
