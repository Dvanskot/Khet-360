using System.Collections.Generic;
using Khet360.Domain.Entities.Common;
using Khet360.Domain.Entities.Platform;
using Khet360.Domain.Entities.Tenant;
using System.IO;
using System.Threading.Tasks;

namespace Khet360.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder);
    Task<Stream> DownloadFileAsync(string fileKey);
    Task DeleteFileAsync(string fileKey);
    Task<IEnumerable<string>> ListFilesAsync(string folder);
    Task<string> GetPresignedUrlAsync(string fileKey, int expiryMinutes = 60);
}
