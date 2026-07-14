namespace GovUk.Forms.Components.Binding;

public interface IContentBinderFactory
{
    IContentBinder Create(string typeName);
}