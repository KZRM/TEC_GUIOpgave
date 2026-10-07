using Feedr.Models;

namespace Feedr.Services;

public class FileService
{
    public const long MaxFileSize = 10 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = [".txt", ".pdf", ".docx", ".csv"];
    private readonly string folder;

    public FileService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        // 2.3: Uploads ligger uden for wwwroot og udleveres kun gennem API'et.
        folder = Path.GetFullPath(configuration["FileStorage:Path"] ?? "App_Data/Uploads", environment.ContentRootPath);
        Directory.CreateDirectory(folder);
    }

    public StoredFile[] ListFiles()
    {
        // 2.1: LINQ læser, sorterer og beskriver filerne fra uploadmappen.
        return new DirectoryInfo(folder).EnumerateFiles()
            .Where(file => file.Name.Length > 33 && file.Name[32] == '_'
                && Guid.TryParseExact(file.Name[..32], "N", out _))
            .OrderByDescending(file => file.LastWriteTimeUtc)
            .Select(Describe)
            .ToArray();
    }

    public async Task<StoredFile> SaveAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var name = file.FileName;

        // 2.3: Serveren kontrollerer input, selv om klienten også validerer.
        if (file.Length == 0 || file.Length > MaxFileSize)
            throw new ArgumentException("Filen skal indeholde data og må højst fylde 10 MB.");
        if (string.IsNullOrWhiteSpace(name) || name.Length > 150
            || name.Any(character => char.IsControl(character) || "<>:\"/\\|?*".Contains(character)))
            throw new ArgumentException("Filnavnet er ugyldigt eller længere end 150 tegn.");
        if (!AllowedExtensions.Contains(Path.GetExtension(name), StringComparer.OrdinalIgnoreCase))
            throw new ArgumentException("Vælg en .txt-, .pdf-, .docx- eller .csv-fil.");

        // Et nyt ID forhindrer overskrivning af filer med samme navn.
        var id = Guid.NewGuid();
        var temporaryPath = Path.Combine(folder, $"{id:N}.tmp");
        var destination = Path.Combine(folder, $"{id:N}_{name}");
        try
        {
            // 2.2.6: Asynkron skrivning; kun færdige uploads vises i oversigten.
            await using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write,
                FileShare.None, 81920, FileOptions.Asynchronous))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }
            File.Move(temporaryPath, destination);
            return Describe(new FileInfo(destination));
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    public FileInfo? Find(Guid id)
    {
        // 2.3: Et GUID bruges til opslag, så klienten ikke kan vælge en filsti.
        return Directory.EnumerateFiles(folder, $"{id:N}_*")
            .Select(path => new FileInfo(path))
            .FirstOrDefault(file => file.Exists);
    }

    public bool Delete(Guid id)
    {
        var file = Find(id);
        if (file is null) return false;
        file.Delete();
        return true;
    }

    private static StoredFile Describe(FileInfo file) => new(
        Guid.ParseExact(file.Name[..32], "N"), file.Name[33..], file.Length, file.LastWriteTimeUtc);
}
