using System.Reflection;
using Demo.GovUk.Forms.AboutYou.Factories;
using Demo.GovUk.Forms.Bankruptcy.Factories;
using Demo.GovUk.Forms.Business.Factories;
using GovUk.Forms.Components.Factories;
using GovUk.Forms.Domain;
using GovUk.Forms.Domain.Serialization;
using Inss.GovUk.Forms.IPUpload.Factories;

List<Assembly> _ =
[
    typeof(FullNameModel).Assembly,
    typeof(AboutYouFormFactory).Assembly,
    typeof(BankruptcyFormFactory).Assembly,
    typeof(IPUploadFormFactory).Assembly,
    typeof(BusinessFormFactory).Assembly
];

if (Directory.Exists("Forms"))
{
    Directory.Delete("Forms", true);
}

Directory.CreateDirectory("Forms");

foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName?.Contains("GovUk.Forms") == true))
{
    Type[] factoryTypes = assembly.GetTypes().Where(t => !t.IsInterface && typeof(IFormFactory).IsAssignableFrom(t)).ToArray();
    
    foreach (Type factoryType in factoryTypes)
    {
        Console.WriteLine(factoryType.FullName);

        IFormFactory formFactory = (IFormFactory)Activator.CreateInstance(factoryType, [])!;
        FormModel form = formFactory.Create();
        
        string json = FormSerializer.SerializeForm(form);
        string fileName = form.Path.Value.Replace("/", "");
        File.WriteAllText($"Forms/{fileName}.json", json);
    }
}

Console.WriteLine("Done.");