using GovUk.Forms.Domain.Primitives;

namespace GovUk.Forms.Components;

public interface IWebRoot
{
    ContentPath Root { get; }
    
    string Name { get; }
}