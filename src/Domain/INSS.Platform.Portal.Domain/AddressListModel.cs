namespace INSS.Platform.Portal.Domain;

public class AddressListModel : PageModel
{    
    private AddressModel? _addressModel;

    public AddressListModel()
    {
        PathName = "address-list";
        Title = "Address List";
        Controller = "AddressList";
    }

    public List<AddressModel> AddressList { get; set; } = [];

    public void AddAddress(AddressModel address)
    {
        _addressModel = address;
    }

    public void AppendAddressToList()
    {
        if (_addressModel is not null)
        {
            AddressList.Add(CloneAddressModel()!);
            ClearAddressModel();
        }
    }

    private AddressModel? CloneAddressModel()
    {
        return _addressModel is null
            ? null
            : new AddressModel
        {
            AddressLine1 = _addressModel.AddressLine1,
            AddressLine2 = _addressModel.AddressLine2,
            TownCity = _addressModel.TownCity,
            County = _addressModel.County,
            Postcode = _addressModel.Postcode,
            Title = _addressModel.Title,
            PageUrl = _addressModel.PageUrl,
            NextPageUrl = _addressModel.NextPageUrl,
            PreviousPageUrl = _addressModel.PreviousPageUrl,
            Controller = _addressModel.Controller,
            Action = _addressModel.Action
        };
    }

    private void ClearAddressModel()
    {
        _addressModel?.AddressLine1 = string.Empty;
        _addressModel?.AddressLine2 = string.Empty;  
        _addressModel?.TownCity = string.Empty;  
        _addressModel?.County = string.Empty;
        _addressModel?.Postcode = string.Empty;
    }
}