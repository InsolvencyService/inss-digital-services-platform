namespace GovUk.Forms.Domain.Search;

public sealed class SearchResultDetailModel : PageModel
{
    public SearchResult Result { get; set; }
    
    public SearchDefinition Definition { get; set; }

    public (SearchCategory Category, CategorizedSearchResultDetail[] Info)[] GetCategorizedResults()
    {
        List<(SearchCategory Category, CategorizedSearchResultDetail[] Info)> categoryInfoList = [];
        
        foreach (SearchCategory category in Definition.Categories)
        {
            List<CategorizedSearchResultDetail> categoryDetailList = [];
            
            foreach (SearchDefinitionField categoryField in Definition.Fields.Where(f => f.Category == category.Label).OrderBy(f => f.Order))
            {
                KeyValuePair<string, string>[] fields = Result.Fields.Where(f => categoryField.Names.Contains(f.Key)).ToArray();
                
                categoryDetailList.Add(new CategorizedSearchResultDetail
                {
                    Label = categoryField.Header, 
                    Value = categoryField.GetFormattedValue(fields.Select(f => f.Value).ToArray())
                });
            }

            categoryInfoList.Add((category, categoryDetailList.ToArray()));
        }

        return categoryInfoList.ToArray();
    }
}