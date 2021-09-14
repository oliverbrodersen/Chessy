
namespace Chessy.Models.Board;
public abstract class Piece
{
    public PieceType Type {  get; set; }

    public Color Color {  get; set; }

    public bool FirstMove {  get; set; }

    public Cell Position {  get; set; }

    public int Value {  get; set; }

    public List<int[]> MovementPattern {  get; set; }

    public char Letter {  get; set; }

    public Cell LastCell {  get; set; }

    public bool Selected {  get; set; }

    public bool Possible {  get; set; }
    
    public bool Castles {  get; set; }
    
    public bool Checked {  get; set; }

    protected Piece(Color color, Cell position, bool firstMove = true)
    {
        Color = color;
        FirstMove = firstMove;
        Position = position;
    }

    internal void Deselect(bool extended)
    {
        Selected = false;
        Possible = false;
        Castles = false;
        if (extended)
        {
            Checked = false;
        }
    }

    internal bool On(Cell cell)
    {
        return cell.Row == Position.Row && cell.Col == Position.Col;
    }
    
    internal void MoveToCell(Cell cell)
    {
        LastCell = Position;
        Position = cell;
    }

    internal string Icon()
    {
        return Color + "_" + Type;
    }
}
