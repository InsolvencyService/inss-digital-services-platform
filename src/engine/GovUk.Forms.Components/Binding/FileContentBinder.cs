using System.Globalization;
using GovUk.Forms.Components.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace GovUk.Forms.Components.Binding;

public sealed class FileContentBinder : DefaultContentBinder
{
    public FileContentBinder(ITypeNameResolver typeNameResolver) : base(typeNameResolver)
    {
    }
    
    protected override Dictionary<string, StringValues> GetFormData(IFormCollection formCollection)
    {
        Dictionary<string, StringValues> formDictionary = formCollection.ToDictionary();
        IFormFileCollection files = formCollection.Files;

        if (files.Count > 1)
        {
            throw new NotSupportedException("The form engine does not support multiple files being uploaded.");
        }

        if (files.Count == 1)
        {
            using var memoryStream = new MemoryStream();
            files[0].CopyTo(memoryStream);
            formDictionary.Add("Filename", new StringValues(files[0].FileName));
            formDictionary.Add("Length", new StringValues(files[0].Length.ToString(CultureInfo.InvariantCulture)));
            formDictionary.Add("Contents", new StringValues(Convert.ToBase64String(memoryStream.ToArray())));
        }

        return formDictionary;
    }
}