namespace XsltLive;

public struct FileInfo
{
    public string FilePath { get; private set; }
    public string FileName { get; private set; }
    public string Directory { get; private set; }

    public FileInfo(string filePath)
    {
        this.FilePath = Path.GetFullPath(filePath);
        this.FileName = Path.GetFileName(filePath);
        this.Directory = Path.GetDirectoryName(filePath);
    }

    public bool AreInSameDirectory(FileInfo other)
    {
        return string.Equals(Path.GetDirectoryName(this.FilePath), Path.GetDirectoryName(other.FilePath), StringComparison.Ordinal);
    }

    public async Task<string> ReadFile()
    {
        using var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var streamReader = new StreamReader(stream);
        return await streamReader.ReadToEndAsync();
    }
}