namespace Inss.FormsSubmission.Service.IPUpload;

public interface IMapperFactory
{
    IMapper Create(object model);
}