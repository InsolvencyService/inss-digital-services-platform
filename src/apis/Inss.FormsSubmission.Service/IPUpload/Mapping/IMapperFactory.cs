namespace Inss.FormsSubmission.Service.IPUpload.Mapping;

public interface IMapperFactory
{
    IMapper Create(object model);
}