namespace INSS.Platform.Portal.Domain;

public sealed class AddAnotherModel : BaseModel
{
    public AddAnotherModel()
    {
        PathName = "add-another";
        Name = "Add Another";
    }
    
    public List<BaseModel[]> Items { get; set; } = [];
    
    public string GetChangeUrl(BaseModel item)
    {
        return $"{PageUrl}/change/?itemId={item.Id}";
    }
    
    public string GetRemoveUrl(BaseModel item)
    {
        return $"{PageUrl}/remove/?itemId={item.Id}";
    }

    public string GetAddAnotherUrl()
    {
        return $"{PageUrl}/add/?itemId={Id}";
    }

    public BaseModel CreateNewRow()
    {
        List<BaseModel> instances = [];

        foreach (BaseModel item in Items[0])
        {
            BaseModel newItem = (BaseModel)Activator.CreateInstance(item.GetType())!;
            item.DeepCopyTo(newItem);
            newItem.Id = Guid.NewGuid().ToString("D");
            newItem.Reset();
            instances.Add(newItem);
        }
        
        Items.Add(instances.ToArray());

        return instances.First();
    }
}