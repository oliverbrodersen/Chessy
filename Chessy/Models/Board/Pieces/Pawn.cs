
namespace Chessy.Models.Board.Pieces;
public class Pawn : Piece
{

    public Pawn (Color color, Cell position, bool firstMove = true) : base(color, position, firstMove)
    {
        Type = PieceType.Pawn;
        Value = 1;
        Letter = 'P';
        MovementPattern = new List<int[]>();
        MovementPattern.Add(new int[] { ((int)Color),0 });
    }
}
