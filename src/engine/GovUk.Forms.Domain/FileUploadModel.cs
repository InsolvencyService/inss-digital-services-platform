namespace GovUk.Forms.Domain;

public sealed class FileUploadModel : PageModel
{
    public FileUploadModel()
    {
        EncodingType = "multipart/form-data";
    }
    
    public string Filename { get; set; }
    
    public string Contents { get; set; }
    
    public int Length { get; set; }
    
    public int SizeInMb => Length / (1024 * 1024);
    
    public override string[] GetSummaryInfo()
    {
        return [Filename];
    }

    public override void CopyTo(PageModel target)
    {
        FileUploadModel xmlFileUpload = target.As<FileUploadModel>();
        xmlFileUpload.Filename = Filename;
        xmlFileUpload.Contents = Contents;
        xmlFileUpload.Length = Length;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Filename = string.Empty;
        Contents = string.Empty;
    }
}