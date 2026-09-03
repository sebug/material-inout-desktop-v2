using SQLite;
namespace material_inout_desktop_v2.Entities;

public class Article
{
    [PrimaryKey]
    [AutoIncrement]
    public int ArticleID { get; set; }

    [Unique]
    public string EAN { get; set; } = String.Empty;
    public string Mnemonic { get; set; } = String.Empty;
    public string Label { get; set; } = String.Empty;
}