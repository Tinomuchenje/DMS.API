namespace DMS.API.Helpers;

public static class FileHelper
{
    public static async Task<string> SaveFileAsync(IFormFile file, string basePath)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        // Ensure directory exists
        Directory.CreateDirectory(basePath);

        // Create unique filename
        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(basePath, fileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return filePath;
    }
}