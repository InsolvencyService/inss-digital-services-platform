namespace INSS.Platform.Auth.API.Tests
{
    internal static class TestHelper
    {
        internal static string GetKeyPem(string keyFileName)
        {
            string keyFilePath = Path.Combine(AppContext.BaseDirectory, keyFileName);
            if (File.Exists(keyFilePath))
            {
                return File.ReadAllText(keyFilePath);
            }

            return string.Empty;
        }
    }
}
