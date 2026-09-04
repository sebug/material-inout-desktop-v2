using material_inout_desktop_v2.Models;

namespace material_inout_desktop_v2.Services;

public interface IArticlesListReader
{
    List<ArticleLine> ReadExcelFile(byte[] bytes);
}