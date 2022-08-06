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
        RockySnow,
        Snow
    }

    public PW_CellInfo(Type cellType = Type.Unknown)
    {
        _cellType = cellType;
    }

    private readonly Type _cellType;
    public Type GetCellType { get => _cellType; }
}
