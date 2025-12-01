namespace INSS.Platform.Portal.Application.Services;

public interface IModelTypeService
{
    Type GetModelType(string name);
    IEnumerable<Type> GetModelTypes();
}