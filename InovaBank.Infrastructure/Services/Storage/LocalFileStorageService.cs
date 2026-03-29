using InovaBank.Domain.Interfaces;

namespace InovaBank.Infrastructure.Services.Storage;

public sealed class LocalFileStorageService : IFileStorageService
{
    private const string StorageFolder = "wwwroot/documents";

    public async Task<string> UploadAsync(string base64Image, string fileName, CancellationToken ct)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), StorageFolder);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var filePath = Path.Combine(path, fileName);
        var bytes = Convert.FromBase64String(base64Image);

        await File.WriteAllBytesAsync(filePath, bytes, ct);

        return $"{StorageFolder}/{fileName}";
    }
}
