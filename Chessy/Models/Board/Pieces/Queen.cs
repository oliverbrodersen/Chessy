
namespace Chessy.Models.Board.Pieces;
public class Queen : Piece
{
    public Queen(Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.Queen;
        Value = 10;
        Letter = 'Q';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { 1, 0 });
        MovementPattern.Add(new int[] { -1, 0 });
        MovementPattern.Add(new int[] { 0, 1 });
        MovementPattern.Add(new int[] { 0, -1 });
        MovementPattern.Add(new int[] { 1, 1 });
        MovementPattern.Add(new int[] { -1, 1 });
        MovementPattern.Add(new int[] { -1, -1 });
        MovementPattern.Add(new int[] { 1, -1 });
    }
}
