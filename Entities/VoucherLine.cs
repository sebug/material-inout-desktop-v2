using SQLite;

namespace material_inout_desktop_v2.Entities;

public class VoucherLine
{
    [PrimaryKey, AutoIncrement]
    public int VoucherLineID { get; set; }

    public int VoucherID { get; set; }

    // Implemented without explicit link to article line so that
    // we can rename, move articles without reference constraints

    public string EAN { get; set; } = String.Empty;

    public string Label { get; set; } = String.Empty;

    public string ReturnStatus { get; set; } = String.Empty;
}
