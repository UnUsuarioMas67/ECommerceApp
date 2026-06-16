using ECommerce.Api.Errors;
using ECommerce.Api.Shared;

namespace ECommerce.Api.Services.Files;

public class ImageService(IWebHostEnvironment environment)
{
    private const int MaxFileSize = 1024 * 1024 * 5;
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];

    public async Task<Result<string>> SaveImageAsync(IFormFile imageFile)
    {
        ArgumentNullException.ThrowIfNull(imageFile);

        var extension = Path.GetExtension(imageFile.FileName);
        if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            return new ImageFileError("Invalid image format");

        if (imageFile.Length > MaxFileSize)
            return new ImageFileError(
                $"File size exceeds the maximum allowed file size. ({MaxFileSize / 1024 / 1024}MB)");

        var imageDir = Path.Combine(environment.WebRootPath, "Images", "Products");
        if (!Directory.Exists(imageDir))
            Directory.CreateDirectory(imageDir);

        var fileName = Guid.NewGuid() + extension;
        var filePath = Path.Combine(imageDir, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await imageFile.CopyToAsync(stream);

        return fileName;
    }

    public void DeleteImage(string fileNameWithExtension)
    {
        if (string.IsNullOrWhiteSpace(fileNameWithExtension))
            throw new ArgumentNullException(nameof(fileNameWithExtension));

        var filePath = Path.Combine(environment.WebRootPath, "Images", "Products", fileNameWithExtension);
        
        if (File.Exists(filePath))
            File.Delete(filePath);
    }
}