namespace Feedr.Models;

public record StoredFile(Guid Id, string Name, long Size, DateTime UploadedAtUtc);
