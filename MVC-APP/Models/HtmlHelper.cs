using HtmlAgilityPack;
namespace MVCAPP.Models;


public class HtmlHelper
{
    public static string GetPlainTextFromHtml(string html)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(html);

        return doc.DocumentNode.InnerText;
    }
}
