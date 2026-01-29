using INSS.Platform.Portal.Domain.Abstract;

namespace INSS.Platform.AlphaDemo.Web.Models;

/// <summary>
/// Represents a list model for income entries, used in forms.
/// </summary>
public class IncomeListModel : FormBase, IFormListModel<IncomeModel>
{
    /// <summary>
    /// Gets or sets the collection of income items.
    /// </summary>
    public List<IncomeModel> Items { get; set; } = new();
}
