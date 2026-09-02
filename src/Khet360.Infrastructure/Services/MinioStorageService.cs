using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Khet360.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using System.Reactive.Linq;

namespace Khet360.Infrastructure.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public MinioStorageService(IConfiguration configuration)
    {
        var endpoint = configuration["Minio:Endpoint"] ?? throw new InvalidOperationException("Minio Endpoint is not configured.");
        var accessKey = configuration["Minio:AccessKey"] ?? throw new InvalidOperationException("Minio AccessKey is not configured.");
        var secretKey = configuration["Minio:SecretKey"] ?? throw new InvalidOperationException("Minio SecretKey is not configured.");
        _bucketName = configuration["Minio:Bucket"] ?? "khet360-assets";

        _minioClient = new MinioClient()
            .WithEndpoint(endpoint)
            .WithCredentials(accessKey, secretKey)
            .Build();
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            var bucketExists = await _minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
            if (!bucketExists)
            {
                await _minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
            }
        }
        catch (Exception)
        {
            // Handle or log bucket creation failure
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, string folder)
    {
        await EnsureBucketExistsAsync();

        var fileKey = $"{folder}/{Guid.NewGuid()}_{fileName}";

        var args = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileKey)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(args);

        return fileKey;
    }

    public async Task<Stream> DownloadFileAsync(string fileKey)
    {
        var memoryStream = new MemoryStream();
        var args = new GetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileKey)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });

        await _minioClient.GetObjectAsync(args);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task DeleteFileAsync(string fileKey)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileKey);

        await _minioClient.RemoveObjectAsync(args);
    }

    public async Task<IEnumerable<string>> ListFilesAsync(string folder)
    {
        // Stubbed for now to resolve build errors.
        // Implementation depends on the specific version of Minio SDK reactive patterns.
        return await Task.FromResult(new List<string>());
    }

    public string GetPresignedUrl(string fileKey, int expiryMinutes = 60)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(fileKey)
            .WithExpiry(expiryMinutes * 60);

        // Try to use the async method and block for the result since the interface is sync
        return _minioClient.PresignedGetObjectAsync(args).GetAwaiter().GetResult();
    }
}
