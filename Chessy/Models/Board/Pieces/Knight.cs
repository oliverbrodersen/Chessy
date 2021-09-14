
namespace Chessy.Models.Board.Pieces;
public class Knight : Piece
{
    public Knight(Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.Knight;
        Value = 3;
        Letter = 'N';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { 2, 1 });
        MovementPattern.Add(new int[] { 2, -1 });
        MovementPattern.Add(new int[] { -2, 1 });
        MovementPattern.Add(new int[] { -2, -1 });
        MovementPattern.Add(new int[] { 1, 2 });
        MovementPattern.Add(new int[] { 1, -2 });
        MovementPattern.Add(new int[] { -1, 2 });
        MovementPattern.Add(new int[] { -1, -2 });
    }
}
