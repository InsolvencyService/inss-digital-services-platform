namespace INSS.Platform.Portal.Domain;

public sealed class AddAnotherModel : BaseModel
{
    public AddAnotherModel()
    {
        PathName = "add-another";
        Name = "Add Another";
    }
    
    //public BaseModel[] Pages { get; set; } = [];

    public List<BaseModel[]> Items { get; set; } = [];

    public AddAnotherActionMode CurrentAction { get; set; }

    public string? CurrentEditId { get; set; }
    
    /*public BaseModel GetNextPage(string id)
    {
        if (CurrentAction == AddAnotherActionMode.Add)
        {
            BaseModel[] items = Items.Last();
            
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Id == id && i < items.Length - 1)
                {
                    return items[i + 1];
                }
            }

            CurrentAction = AddAnotherActionMode.Summary;
            return this;
        }
        else if (CurrentAction == AddAnotherActionMode.Edit)
        {
            foreach (BaseModel[] items in Items)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Id == id && i < items.Length - 1)
                    {
                        CurrentEditId = items[i + 1].Id;
                        return items[i + 1];
                    }
                }
            }

            CurrentAction = AddAnotherActionMode.Summary;
            CurrentEditId = null;
            return this;
        }
        // else if (CurrentAction == AddAnotherActionMode.Remove)
        // {
        //     CurrentAction = Items.Count == 0 ? AddAnotherActionMode.Edit : AddAnotherActionMode.Summary;
        //
        //     if (CurrentAction == AddAnotherActionMode.Edit)
        //     {
        //         return Items[0][0];
        //     }
        // }
        
        return this;
    }
    
    public BaseModel GetNextPage(BaseModel currentPage)
    {
        foreach (BaseModel[] items in Items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Id == currentPage.Id && i < items.Length - 1)
                {
                    return items[i + 1];
                }
            }
        }
        
        // for (int i = 0; i < Pages.Length; i++)
        // {
        //     if (Pages[i].Id == currentPage.Id && i < Pages.Length - 1)
        //     {
        //         return Pages[i + 1];
        //     }
        // }

        return this;
    }
    
    public BaseModel GetFirstPage()
    {
        return Items[0][0];
        //return Pages.First();
    }*/
    
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
        //List<Type> types = Items[0].Select(i => i.GetType()).ToList();

        List<BaseModel> instances = [];

        CurrentAction = AddAnotherActionMode.Add;
        
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

public enum AddAnotherActionMode
{
    Add,
    Edit,
    Remove,
    Summary
}