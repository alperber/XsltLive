using System.Text.Json;
using XsltLive;

try
{
    AppConfig.Initialize(args);
    var ws = new WebsocketServer();
    var httpServer = new HttpServer();
    var renderer = new XsltRenderer();
    await renderer.SetRenderedHtml();
    
#pragma warning disable CS4014
    Task.Run(() => ws.Start());
#pragma warning restore CS4014
    
    var xmlWatcher = new FileSystemWatcher(AppConfig.XmlFileInfo.Directory);
    xmlWatcher.NotifyFilter = NotifyFilters.LastWrite;
    xmlWatcher.Filter = AppConfig.XmlFileInfo.FileName;
    xmlWatcher.IncludeSubdirectories = false;
    xmlWatcher.EnableRaisingEvents = true;
    
    var xsltWatcher = new FileSystemWatcher(AppConfig.XsltFileInfo.Directory);
    xsltWatcher.NotifyFilter = NotifyFilters.LastWrite;
    xsltWatcher.Filter = AppConfig.XsltFileInfo.FileName;
    xsltWatcher.IncludeSubdirectories = false;
    xsltWatcher.EnableRaisingEvents = true;
    
    xmlWatcher.Changed += OnFileChanged;
    xsltWatcher.Changed += OnFileChanged;
    
    await httpServer.Start();

    void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            xmlWatcher.EnableRaisingEvents = false;
            xsltWatcher.EnableRaisingEvents = false;
            
            if (e.ChangeType != WatcherChangeTypes.Changed) return;
            if(e.Name is not null && !e.Name.Equals(AppConfig.XmlFileInfo.FileName) && !e.Name.Equals(AppConfig.XsltFileInfo.FileName)) return;

            renderer.SetRenderedHtml()
                .ContinueWith(_ =>
                {
                    Logger.LogInfo($"File changed - {e.Name}");
                    ws.NotifyClients();
                });
        }
        finally
        {
            xmlWatcher.EnableRaisingEvents = true;
            xsltWatcher.EnableRaisingEvents = true;
        }
    }
}
catch (Exception ex)
{
    Logger.LogError("Unexpected error when starting XsltLive", ex);
}