using KresApp.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace KresApp.Infrastructure.Services;

public class LocalStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string UploadsFolder = "uploads";

    public LocalStorageService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;

        // Ensure uploads directory exists
        var path = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), UploadsFolder);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), UploadsFolder);
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return BuildUrl(uniqueFileName);
    }

    public async Task<string> UploadFileToFolderAsync(Stream fileStream, string folder, string fileName, string contentType)
    {
        var safeFolder = System.Text.RegularExpressions.Regex.Replace(folder, @"[^\w\-]", "_");
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        
        var uploadsPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), UploadsFolder, safeFolder);
        if (!Directory.Exists(uploadsPath))
        {
            Directory.CreateDirectory(uploadsPath);
        }

        var filePath = Path.Combine(uploadsPath, uniqueFileName);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(stream);

        return BuildUrl($"{safeFolder}/{uniqueFileName}");
    }

    public Task DeleteFileAsync(string fileUrl)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var relativePath = uri.LocalPath.TrimStart('/');
            // Remove 'uploads/' from the start
            if (relativePath.StartsWith(UploadsFolder, StringComparison.OrdinalIgnoreCase))
            {
                var fullPath = Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"), relativePath);
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
            }
        }
        catch { /* Log error */ }
        return Task.CompletedTask;
    }

    private string BuildUrl(string path)
    {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) return $"/{UploadsFolder}/{path}";

        var baseUrl = $"{request.Scheme}://{request.Host}";
        return $"{baseUrl}/{UploadsFolder}/{path}";
    }
}
