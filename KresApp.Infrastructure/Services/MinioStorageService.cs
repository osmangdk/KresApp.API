using KresApp.Application.Interfaces;
using KresApp.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KresApp.Infrastructure.Services;

public class MinioStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;

    public MinioStorageService(IOptions<MinioSettings> settings)
    {
        _settings = settings.Value;
        
        _minioClient = new MinioClient()
            .WithEndpoint(_settings.Endpoint)
            .WithCredentials(_settings.AccessKey, _settings.SecretKey)
            .WithSSL(_settings.UseSsl)
            .Build();
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        // Dosya adını benzersiz yapalım
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        
        // Bucket var mı kontrol et (basitlik için direkt yükleme denenebilir ama kontrol daha iyi)
        var beArgs = new BucketExistsArgs().WithBucket(_settings.BucketName);
        bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
        if (!found)
        {
            var mbArgs = new MakeBucketArgs().WithBucket(_settings.BucketName);
            await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
        }

        // Dosyayı yükle
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_settings.BucketName)
            .WithObject(uniqueFileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

        // Erişim URL'ini oluştur (Minio için genelde endpoint/bucket/object formatındadır)
        var protocol = _settings.UseSsl ? "https" : "http";
        return $"{protocol}://{_settings.Endpoint}/{_settings.BucketName}/{uniqueFileName}";
    }

    public async Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            // URL'den object name'i ayıkla
            var uri = new Uri(fileUrl);
            var fileName = Path.GetFileName(uri.LocalPath);

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(_settings.BucketName)
                .WithObject(fileName);

            await _minioClient.RemoveObjectAsync(removeObjectArgs).ConfigureAwait(false);
        }
        catch
        {
            // Log error
        }
    }
}
