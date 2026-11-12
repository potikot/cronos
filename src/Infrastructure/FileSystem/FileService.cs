namespace Cronos.Infrastructure.FileSystem;

public class FileService : IFileService
{
    // Sync
    
    public bool Write(string path, string data)
    {
        if (!TryCreateDirectory(path)) return false;
        File.WriteAllText(path, data);
        return true;
    }

    public bool WriteBytes(string path, byte[] data)
    {
        if (!TryCreateDirectory(path)) return false;
        File.WriteAllBytes(path, data);
        return true;
    }

    public bool WriteLines(string path, string[] data)
    {
        if (!TryCreateDirectory(path)) return false;
        File.WriteAllLines(path, data);
        return true;
    }

    public string Read(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return File.ReadAllText(path);
    }

    public byte[] ReadBytes(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return File.ReadAllBytes(path);
    }

    public string[] ReadLines(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return File.ReadAllLines(path);
    }
    
    // Async
    
    public async Task<bool> WriteAsync(string path, string data)
    {
        if (!TryCreateDirectory(path)) return false;
        await File.WriteAllTextAsync(path, data);
        return true;
    }

    public async Task<bool> WriteBytesAsync(string path, byte[] data)
    {
        if (!TryCreateDirectory(path)) return false;
        await File.WriteAllBytesAsync(path, data);
        return true;
    }
    
    public async Task<bool> WriteLinesAsync(string path, string[] data)
    {
        if (!TryCreateDirectory(path)) return false;
        await File.WriteAllLinesAsync(path, data);
        return true;
    }

    public async Task<string> ReadAsync(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return await File.ReadAllTextAsync(path);
    }

    public async Task<byte[]> ReadBytesAsync(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return await File.ReadAllBytesAsync(path);
    }

    public async Task<string[]> ReadLinesAsync(string path)
    {
        if (!File.Exists(path))
            return null;
        
        return await File.ReadAllLinesAsync(path);
    }

    private bool TryCreateDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
            return false;

        string dirPath = Path.GetDirectoryName(path);
        if (string.IsNullOrEmpty(dirPath) || Directory.Exists(dirPath))
            return true;
        
        return Directory.CreateDirectory(dirPath).Exists;
    }
}