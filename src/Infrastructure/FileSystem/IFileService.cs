namespace Cronos.Infrastructure.FileSystem;

public interface IFileService
{
    bool Write(string path, string data);
    bool WriteBytes(string path, byte[] data);
    bool WriteLines(string path, string[] data);
    
    string Read(string path);
    byte[] ReadBytes(string path);
    string[] ReadLines(string path);
    
    Task<bool> WriteAsync(string path, string data);
    Task<bool> WriteBytesAsync(string path, byte[] data);
    Task<bool> WriteLinesAsync(string path, string[] data);
    
    Task<string> ReadAsync(string path);
    Task<byte[]> ReadBytesAsync(string path);
    Task<string[]> ReadLinesAsync(string path);
}