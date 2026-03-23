namespace GovUk.Forms.Domain;

public class RemoveModel : PageModel
{
    public string RemoveQuestion { get; set; } = "Confirm Remove?";
    
    public string RemoveHint { get; set; } = "Select one option";
    
    public bool RemoveConfirmed { get; set; }
    
    public int SetIndex { get; set; } = -1;
    
    public void ResetRemove()
    {
        SetIndex = -1;
        RemoveConfirmed = false;
        ReturnUrl = null;
    }
}