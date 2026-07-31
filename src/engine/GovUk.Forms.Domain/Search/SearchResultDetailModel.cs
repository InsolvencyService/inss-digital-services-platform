using System.Reflection.Emit;

namespace GovUk.Forms.Domain.Search;

public sealed class SearchResultDetailModel : PageModel
{
    public SearchResult Result { get; set; }
    
    public SearchDefinition Definition { get; set; }

    public (SearchCategory Category, CategorizedSearchResultDetail[] Info)[] GetCategorizedResults()
    {
        List<(SearchCategory Category, CategorizedSearchResultDetail[] Info)> categoryInfoList = [];

        List<AuthorisingBodyLookup> authorisingBodies = Definition.AuthorisingBodies.ToList();

        string authorisingBodyCode = "";

        foreach (SearchCategory category in Definition.Categories)
        {
            List<CategorizedSearchResultDetail> categoryDetailList = [];
            
            foreach (SearchDetailDefinition categoryField in Definition.Details.Where(d => d.Category == category.Label).OrderBy(f => f.Order))
            {
                KeyValuePair<string, string>[] fields = Result.Fields.Where(f => categoryField.Names.Contains(f.Key)).ToArray();

                // Get Authorising Body details for this Insolvency Practitioner from config (if available)
                if (string.IsNullOrWhiteSpace(authorisingBodyCode))
                {
                    authorisingBodyCode = fields.FirstOrDefault(f => f.Key == "LicensingBody").Value;
                }

                if (categoryField.Category == "Authorising body" && !string.IsNullOrEmpty(authorisingBodyCode))
                {
                    AuthorisingBodyLookup? authorisingBody = authorisingBodies.FirstOrDefault(abc => abc.AuthBodyCode == authorisingBodyCode);

                    List<string> categoryFieldLabels = categoryField.Names.ToList();
                    string[] categoryFields = categoryFieldLabels.ToArray();

                    string categoryFieldValues = string.Join(", ",
                        categoryFields.Select(fieldName =>
                        authorisingBody?.GetType().GetProperty(fieldName)?.GetValue(authorisingBody)?.ToString()
                        ?? string.Empty).Where(value => !string.IsNullOrEmpty(value)));

                    categoryDetailList.Add(new CategorizedSearchResultDetail
                    {
                        Label = categoryField.GetLabel(),
                        Value = categoryFieldValues
                    });
                }
                else
                {
                    categoryDetailList.Add(new CategorizedSearchResultDetail
                    {
                        Label = categoryField.GetLabel(),
                        Value = categoryField.GetValue(fields.Select(f => f.Value).ToArray())
                    });
                }
            }

            categoryInfoList.Add((category, categoryDetailList.ToArray()));
        }

        return categoryInfoList.ToArray();
    }
}