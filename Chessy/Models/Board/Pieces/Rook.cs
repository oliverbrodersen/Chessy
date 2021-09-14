
namespace Chessy.Models.Board.Pieces;
public class Rook : Piece
{

    public Rook(Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.Rook;
        Value = 5;
        Letter = 'R';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { 1, 0 });
        MovementPattern.Add(new int[] { -1, 0 });
        MovementPattern.Add(new int[] { 0, 1 });
        MovementPattern.Add(new int[] { 0, -1 });
    }
}
