namespace XsltLive;

public class XsltRenderer
{
    public async Task SetRenderedHtml()
    {
        var invoiceXml = await AppConfig.XmlFileInfo.ReadFile();
        var xslt = await AppConfig.XsltFileInfo.ReadFile();

        var xsltTransform = new System.Xml.Xsl.XslCompiledTransform();
        xsltTransform.Load(new System.Xml.XmlTextReader(new System.IO.StringReader(xslt)));

        var xmlDocument = new System.Xml.XmlDocument();
        xmlDocument.LoadXml(invoiceXml);

        var stringWriter = new System.IO.StringWriter();
        xsltTransform.Transform(xmlDocument, null, stringWriter);

        var reloadScript = AppConfig.ReloadScript;
        GlobalState.RenderedHtml = stringWriter.ToString().Replace("</body>", reloadScript + "</body>");
    }
}