using System.IO;
using System.Threading.Tasks;

namespace KresApp.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
    Task<string> UploadFileToFolderAsync(Stream fileStream, string folder, string fileName, string contentType);
    Task DeleteFileAsync(string fileUrl);
}
