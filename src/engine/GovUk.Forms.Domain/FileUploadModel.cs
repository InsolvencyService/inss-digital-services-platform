using GovUk.Forms.Domain.Attributes;
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace GovUk.Forms.Domain;

public class FileUploadModel : PageModel
{
    public FileUploadModel()
    {
        ViewName = "_FileUpload";
    }
    
    [Summary]
    [Copyable]
    public string Filename { get; set; }
    
    [Copyable]
    public string Contents { get; set; }
    
    public int Length { get; set; }
    
    public int SizeInMb => Length / (1024 * 1024);
}