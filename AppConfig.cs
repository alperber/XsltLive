using System.Text;

namespace XsltLive;

public static class AppConfig
{
    public static FileInfo XmlFileInfo { get; private set; }
    public static FileInfo XsltFileInfo { get; private set; }
    public static int HttpPort { get; private set; }

    public static int WebsocketPort { get; private set; }

    public static byte[] NotFoundResponseInBytes { get; private set; }

    public static byte[] ReloadCommandInBytes { get; private set; }

    public static string ReloadScript { get; private set; }

    public static void Initialize(string[] args)
    {
        NotFoundResponseInBytes = Encoding.UTF8.GetBytes("NOT FOUND");
        ReloadCommandInBytes = Encoding.UTF8.GetBytes("RELOAD");
        ParseAndSetOptions(args);
        SetReloadScript();
    }

    private static void ParseAndSetOptions(string[] args)
    {
        var httpPortOptionLabels = new[] { "--http-port", "-hp" };
        var websocketPortOptionLabels = new[] { "--websocket-port", "-wsp" };
        var xmlOptionLabels = new[] { "--xml-file-path", "-xml" };
        var xsltOptionLabels = new[] { "--xslt-file-path", "-xslt"};

        HttpPort = ParseIntOption(args, httpPortOptionLabels, 5000);
        WebsocketPort = ParseIntOption(args, websocketPortOptionLabels, 5001);

        XmlFileInfo = ParseFileOption(args, xmlOptionLabels, "Xml file is necessary!");
        XsltFileInfo = ParseFileOption(args, xsltOptionLabels, "Xslt file is necessary!");
    }

    private static int ParseIntOption(string[] args, string[] optionLabels, int defaultValue)
    {
        foreach (var label in optionLabels)
        {
            var idx = Array.IndexOf(args, label);

            if (idx == -1 || idx >= args.Length - 2) continue;
            if (int.TryParse(args[idx + 1], out var value))
            {
                return value;
            }
        }

        return defaultValue;
    }

    private static FileInfo ParseFileOption(string[] args, string[] optionLabels, string errorMessage)
    {
        foreach (var label in optionLabels)
        {
            var idx = Array.IndexOf(args, label);

            if (idx == -1 || idx >= args.Length - 1) continue;
            return new FileInfo(args[idx + 1]);
        }
        Logger.LogError(errorMessage);
        Environment.Exit(-1);
        
        // code will not reach here
        return default;
    }
    
    private static void SetReloadScript()
    {
        ReloadScript = @$"
<script>
    function __listenWebsocket__(){{
        try{{
            let ws = new WebSocket('ws://localhost:{WebsocketPort}');
            ws.onmessage = function(event){{
                if(event.data == 'RELOAD'){{
                    window.location.reload();
                }}
            }};
        }}catch(e){{
            console.error(e);
        }}
    }}
    __listenWebsocket__();
</script>
";
    }
}