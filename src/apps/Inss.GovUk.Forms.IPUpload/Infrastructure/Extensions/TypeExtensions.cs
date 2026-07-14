namespace Inss.GovUk.Forms.IPUpload.Infrastructure.Extensions;

public static class TypeExtensions
{
    extension(Type type)
    {
        public Stream GetEmbeddedStream(string key)
        {
            string path = $"Inss.GovUk.Forms.IPUpload.Infrastructure.{key}";
            Stream stream = type.Assembly.GetManifestResourceStream(path) ?? Stream.Null;
            return stream == Stream.Null ? throw new InvalidOperationException($"Unable to find an embedded resource for {key}.") : stream;
        }
    }
}