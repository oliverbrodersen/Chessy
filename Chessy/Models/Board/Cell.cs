using Chessy.Models.Board;

namespace Chessy.Models;
public class Cell
{
    public int Row { get; set; }
    public int Col {  get; set; }
    public bool Possible { get; set; }
    public bool Selected { get; set; }
    public bool Castles { get; set; }
    public bool LastMove {  get; set; }
    public bool Checked {  get; set; }

    public Piece? Piece { get; set; }

    public Cell(int row, int col)
    {
        Row = row;
        Col = col;
    }

    internal Cell Copy()
    {
        var cell = new Cell(Row, Col);
        cell.Piece = Piece;
        return cell;
    }
}
