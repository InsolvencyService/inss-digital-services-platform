using GovUk.Forms.Domain.Exceptions;
using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Domain;

public sealed class SectionModelList : List<SectionModel>
{
    public SectionModel this[ContentPath path]
    {
        get
        {
            SectionModel? section = this.FirstOrDefault(s => s.Path == path);
            return section ?? throw new ModelException($"Unable to find the section using the path {path}.");
        }
    }
    
    public SectionModel this[string title]
    {
        get
        {
            SectionModel? section = this.FirstOrDefault(s => s.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
            return section ?? throw new ModelException($"Unable to find the section using the title {title}.");
        }
    }
}