
namespace Chessy.Models;
public class Piece
{
    public PieceType Type { get; set; }

    public bool Captured { get; set; }

    public bool FirstMove { get; set; }

    public Color Color { get; set; }

    public Piece(PieceType type, Color color)
    {
        Type = type;
        Color = color;
        FirstMove = true;
    }

    public int Points()
    {
        switch (Type)
        {
            case PieceType.Bishop: 
            case PieceType.Knight: return 3; 
            case PieceType.Rook: return 5;
            case PieceType.Queen: return 10;
            case PieceType.Pawn: return 1;
            case PieceType.King: return 15;
        }
        return 0;
    }

    public override string? ToString()
    {
        string c = "";

        switch (Type)
        {
            case PieceType.Bishop:
                c = Color == Color.White ? "♗B" : "♝B"; break;
            case PieceType.Rook:
                c = Color == Color.White ? "♖R" : "♜R"; break;
            case PieceType.Knight:
                c = Color == Color.White ? "♘N" : "♞N"; break;
            case PieceType.King:
                c = Color == Color.White ? "♔K" : "♚K"; break;
            case PieceType.Queen:
                c = Color == Color.White ? "♕Q" : "♛Q"; break;
            case PieceType.Pawn:
                c = Color == Color.White ? "♙" : "♟"; break;
        }

        return c;
    }

    internal string Icon()
    {
        return Color.ToString() + "_" + Type.ToString();
    }
}
