/// <summary>
///     [What does this PW_CellInfo do]
/// </summary>
public sealed class PW_CellInfo
{
    public enum Type 
    {
        Unknown,
        Water,
        Sand,
        Greenery,
        Snow
    }

    private Type _cellType;
    public Type CellType { get => _cellType; set => _cellType = value; }
}
