using System.Text.Json.Serialization;

namespace GovUk.Forms.Domain;

public sealed class AddAnotherModel : PageModel
{
    public bool AddAnotherItem { get; set; }
    
    public PageModelList Items { get; init; } = [];

    public string? Question { get; set; }

    public string? Hint { get; set; }

    public int GroupLength { get; set; }

    public bool CanAddAnother { get; set; } = true;
    
    [JsonIgnore]
    public AddAnotherSummaryModel[] SummaryInfo { get; set; } = [];

    public override void CopyTo(PageModel target)
    {
        AddAnotherModel addAnother = target.As<AddAnotherModel>();
        addAnother.Question =  Question;
        addAnother.Hint = Hint;
        addAnother.GroupLength = GroupLength;
        addAnother.AddAnotherItem = AddAnotherItem;
    }
    
    public override void ClearValues()
    {
        base.ClearValues();
        Question = null;
        Hint = null;
        GroupLength = 0;
        AddAnotherItem = false;
    }

    public override string[] GetSummaryInfo()
    {
        return Items.SelectMany(p => p.GetSummaryInfo()).ToArray();
    }
    
    public sealed class AddAnotherSummaryModel
    {
        public required string Value { get; init; }
        
        public required string ChangeUrl { get; init; }
        
        public required string RemoveUrl { get; init; }
    }
}

