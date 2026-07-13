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
                KeyValuePair<string, string> field = Result.Fields.FirstOrDefault(f => f.Key == categoryField.Name);
                
                categoryDetailList.Add(new CategorizedSearchResultDetail
                {
                    Label = categoryField.Header, 
                    Value = categoryField.GetFormattedValue(field.Value)
                });
            }

            categoryInfoList.Add((category, categoryDetailList.ToArray()));
        }

        return categoryInfoList.ToArray();
    }
}