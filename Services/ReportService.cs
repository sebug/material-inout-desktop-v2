using System.Text;
using material_inout_desktop_v2.Entities;
using material_inout_desktop_v2.Repositories;

namespace material_inout_desktop_v2.Services;

public class ReportService : IReportService
{
    private readonly IArticleRepository ArticleRepository;

    public ReportService(IArticleRepository articleRepository)
    {
        ArticleRepository = articleRepository;
    }

    public async Task<string> GenerateVoucherHTML(int voucherID)
    {
        var voucher = await ArticleRepository.GetVoucherById(voucherID);

        string html = TemplateHtml;
        if (voucher.ReturnedDate.HasValue)
        {
            html = ReturnedTemplateHtml;
        }
        html = html.Replace("{{voucherId}}", voucherID.ToString())
            .Replace("{{name}}", voucher.Name)
            .Replace("{{createdDate}}", voucher.CreatedDate.ToString("dd.MM.yyyy"))
            .Replace("{{logo_url}}", OrganizationLogo.DATA_URL);
        
        if (voucher.ReturnedDate.HasValue)
        {
            html = html.Replace("{{returnedDate}}", voucher.ReturnedDate.Value.ToString("dd.MM.yyyy"))
            .Replace("{{returnedPersonName}}", voucher.ReturningPersonName);
        }


        var voucherLines = await ArticleRepository.GetVoucherLinesByVoucherId(voucherID);
        html = html.Replace("{{lines}}", GetVoucherLinesTable(voucherLines, voucher.ReturnedDate.HasValue));

        return html;
    }

    private string GetVoucherLinesTable(List<VoucherLine> voucherLines, bool includeStatus)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<table>");
        sb.AppendLine("<thead>");
        sb.AppendLine("<tr>");
        sb.AppendLine("<th>EAN</th>");
        sb.AppendLine("<th>Libellé</th>");
        if (includeStatus)
        {
            sb.AppendLine("<th>Statut Retour</th>");
        }
        sb.AppendLine("</tr>");
        sb.AppendLine("</thead>");
        sb.AppendLine("<tbody>");
        foreach (var voucherLine in voucherLines)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>" + voucherLine.EAN + "</td>");
            sb.AppendLine("<td>" + voucherLine.Label + "</td>");
            if (includeStatus)
            {
                sb.AppendLine("<td>" + voucherLine.ReturnStatus + "</td>");
            }
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table>");
        return sb.ToString();
    }

    public string TemplateHtml = @"
    <!DOCTYPE html>
    <html>
    <head>
    <style>
    body {
        font-family: sans-serif;
    }
    th {
        text-align: left;
        padding-left: 1em;
    }
    td {
        padding-left: 1em;
    }
    .signature {
        padding-bottom: 5em;
        border-bottom: 1px solid black;
    }
    .address {
        float: right;
    }
    .logo {
        clear: left;
        float: left;
    }
    h1 {
        clear: both;
    }
    .print-options button
    {
        color: #fff;
        background-color: #aa4400;
        font-size: 18px;
        padding-left: 20px;
        padding-right: 20px;
        padding-top: 20px;
        padding-bottom: 20px;
    }
    @media print {
        .print-options {
            display: none;
        }
    }
    </style>
    </head>
    <body>
    <p class=""print-options""><button class=""print"">Imprimer</button></p>
    <p class=""logo""><img width=""200"" src=""{{logo_url}}"" /></p>
    <p class=""address"">ORPC Valavran,<br />
Rue du Village 27<br />
1294 Genthod<br />
Tél. +41 22 774 08 06</p>
    <h1>Bon de Sortie {{voucherId}}</h1>
    <p>Responsable: {{name}}</p>
    <p>Date de création: {{createdDate}}</p>

    {{lines}}

    <p class=""signature"">Signature</p>

    <script>
    let printButton = document.querySelector('.print');
    printButton.addEventListener('click', function () {
        try {
            window.print();
        } catch (e) {
            alert('Erreur impression');
        }
        alert('Document imprimé');
    });
    </script>
    </body>
    </html>
    ";

    public string ReturnedTemplateHtml = @"
    <!DOCTYPE html>
    <html>
    <head>
    <style>
    body {
        font-family: sans-serif;
    }
    th {
        text-align: left;
        padding-left: 1em;
    }
    td {
        padding-left: 1em;
    }
    .signature {
        padding-bottom: 5em;
        border-bottom: 1px solid black;
    }
    .address {
        float: right;
    }
    .logo {
        clear: left;
        float: left;
    }
    h1 {
        clear: both;
    }
    .print-options button
    {
        color: #fff;
        background-color: #aa4400;
        font-size: 18px;
        padding-left: 20px;
        padding-right: 20px;
        padding-top: 20px;
        padding-bottom: 20px;
    }
    @media print {
        .print-options {
            display: none;
        }
    }
    </style>
    </head>
    <body>
    <p class=""print-options""><button class=""print"">Imprimer</button></p>
    <p class=""logo""><img width=""200"" src=""{{logo_url}}"" /></p>
    <p class=""address"">ORPC Valavran,<br />
Rue du Village 27<br />
1294 Genthod<br />
Tél. +41 22 774 08 06</p>
    <h1>Bon de Retour {{voucherId}}</h1>
    <p>Responsable: {{name}}</p>
    <p>Date de création: {{createdDate}}</p>
    <p>Date de retour: {{returnedDate}}</p>
    <p>Personne confirmant le retour: {{returnedPersonName}}</p>

    {{lines}}

    <p class=""signature"">Signature</p>

    <script>
    let printButton = document.querySelector('.print');
    printButton.addEventListener('click', function () {
        try {
            window.print();
        } catch (e) {
            alert('Erreur impression');
        }
        alert('Document imprimé');
    });
    </script>
    </body>
    </html>
    ";
}
