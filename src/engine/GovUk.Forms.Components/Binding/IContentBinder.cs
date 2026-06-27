using GovUk.Forms.Domain;
using Microsoft.AspNetCore.Http;

namespace GovUk.Forms.Components.Binding;

public interface IContentBinder
{
    ContentModel BindAndReturnModel(string typeName, IFormCollection formCollection, char separator = '.');
}