namespace INSS.Platform.Portal.Domain.Abstract;

/// <summary>
/// Represents a model for a list of form items.
/// </summary>
/// <typeparam name="TFormItem">The type of the form item.</typeparam>
public interface IFormListModel<TFormItem>
{
    /// <summary>
    /// Gets or sets the collection of form items.
    /// </summary>
    List<TFormItem> Items { get; set; }
}