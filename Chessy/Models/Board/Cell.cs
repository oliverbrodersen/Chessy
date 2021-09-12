namespace Chessy.Models;
public class Cell
{
    public int Row { get; set; }
    public int Col {  get; set; }
    public bool Empty { get; set; }
    public bool Possible { get; set; }
    public bool Selected { get; set; }
    public bool Castles { get; set; }
    public bool LastMove {  get; set; }


    public Piece? Piece { get; set; }

    public Cell(int row, int col, Piece piece = null)
    {
        Row = row;
        Col = col;
        Piece = piece;
        Empty = true;
    }

    internal void SetPiece(Piece piece)
    {
        Piece = piece;
        Empty = false;
    }

    internal void RemovePiece()
    {
        Piece = null;
        Empty = true;
    }

    public string Icon()
    {
        return Empty ? "empty" : Piece.Icon();
    }
    public override string? ToString()
    {
        return Empty ? "empty" : Piece.ToString();
    }
}
