using System.Reflection;

namespace Inss.GovUk.Forms.IPUpload.Domain.Validation;

internal static class ValidationInfoLookup
{
    private static readonly Dictionary<string, ValidationInfo> _lookup = [];
    private static readonly Type[] _validationTypes =
    [
        typeof(AddressValidationInfo),
        typeof(AssociatedCompanyValidationInfo),
        typeof(BusinessValidationInfo),
        typeof(CaseValidationInfo),
        typeof(DirectorValidationInfo),
        typeof(EmployeeHolidayValidationInfo),
        typeof(EmployeePayValidationInfo),
        typeof(EmployeeValidationInfo),
        typeof(EmployerValidationInfo),
        typeof(EmploymentContinuityValidationInfo),
        typeof(InsolvencyPractitionerValidationInfo),
        typeof(PayRecordsContactValidationInfo),
        typeof(ShareholderValidationInfo),
        typeof(TransfersValidationInfo)
    ];
    
    static ValidationInfoLookup()
    {
        foreach (Type validationType in _validationTypes)
        {
            FieldInfo[] fields = validationType.GetFields(BindingFlags.Static | BindingFlags.NonPublic);

            foreach (FieldInfo field in fields)
            {
                if (field.GetValue(null) is ValidationInfo validationInfo)
                {
                    _lookup.Add(validationInfo.Key, validationInfo);
                }
            }
        }
    }

    internal static ValidationInfo Get(string key)
    {
        return _lookup[key];
    }
}