namespace Inss.Platform.Submission.Service.IPUpload.Mapping;

public interface IMapperFactory
{
    IMapper Create(object model);
}