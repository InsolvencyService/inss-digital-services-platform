namespace INSS.Platform.Portal.Domain;

public sealed class ItemCollection : List<PageCollection>
{
    public BaseModel CreateNewRow()
    {
        PageCollection instances = [];

        foreach (BaseModel item in this[0])
        {
            BaseModel newItem = (BaseModel)Activator.CreateInstance(item.GetType())!;
            item.DeepCopyTo(newItem);
            newItem.Id = Guid.NewGuid().ToString("D");
            newItem.Reset();
            instances.Add(newItem);
        }
        
        Add(instances);

        return instances.First();
    }
    
    public void UpdateExistingItem(BaseModel updatedModel)
    {
        BaseModel? existingModel = this.SelectMany(i => i).FirstOrDefault(i => i.Id == updatedModel.Id);
        
        if (existingModel is not null)
        {
            updatedModel.CopyTo(existingModel);
        }
    }

    public BaseModel? FindExistingFor(BaseModel model)
    {
        return this.SelectMany(pages => pages.Where(item => item.Id == model.Id && item.PageUrl == model.PageUrl)).FirstOrDefault();
    }
    
    public BaseModel? FindExistingFor(string pageId)
    {
        return this.SelectMany(pages => pages.Where(item => item.Id == pageId)).FirstOrDefault();
    }

    public BaseModel? GetNextPageAfter(BaseModel page, AddAnotherModel addAnother)
    {
        foreach (PageCollection pages in this)
        {
            for (int i = 0; i < pages.Count; i++)
            {
                if (pages[i].Id == page.Id)
                {
                    if (i < pages.Count - 1)
                    {
                        return pages[i + 1];
                    }
                    
                    return addAnother;
                }
            }
        }
        
        return null;
    }
}