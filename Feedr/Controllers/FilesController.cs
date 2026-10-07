using Feedr.Models;
using Feedr.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Feedr.Controllers;

[ApiController]
[Route("api/files")]
[EnableCors("FileClient")]
public class FilesController(FileService files) : ControllerBase
{
    // 2.2.3: Klienten får filoversigten som JSON.
    [HttpGet]
    public ActionResult<StoredFile[]> List() => files.ListFiles();

    // 2.1: POST modtager filen som multipart/form-data.
    [HttpPost]
    [RequestSizeLimit(FileService.MaxFileSize + 64 * 1024)]
    [RequestFormLimits(MultipartBodyLengthLimit = FileService.MaxFileSize)]
    [ProducesResponseType<StoredFile>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StoredFile>> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var saved = await files.SaveAsync(file, cancellationToken);
            return CreatedAtAction(nameof(Download), new { id = saved.Id }, saved);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet("{id:guid}/download")]
    [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK, "application/octet-stream")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Download(Guid id)
    {
        var file = files.Find(id);
        if (file is null) return NotFound(new { message = "Filen findes ikke længere." });

        try
        {
            // 2.2.6: Filen streames som download, også når den er PDF eller DOCX.
            var stream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read,
                FileShare.Read | FileShare.Delete, 81920, FileOptions.Asynchronous);
            return File(stream, "application/octet-stream", file.Name[33..]);
        }
        catch (FileNotFoundException)
        {
            return NotFound(new { message = "Filen findes ikke længere." });
        }
    }
}
