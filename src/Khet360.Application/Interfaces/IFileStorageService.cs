using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task DeleteFileAsync(string fileKey);
    Task<IEnumerable<string>> ListFilesAsync(string folder);
    string GetPresignedUrl(string fileKey, int expiryMinutes = 60);
}
