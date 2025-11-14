using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace INSS.Platform.Portal.Domain;

public class FormModel : BaseModel
{
    private readonly List<string> _navList = [];
    private static JsonSerializerOptions? _options;
    
    public FormModel()
    {
        PathName = "tasks";
        Controller = "Form";
        PageUrl = $"/{PathName}";
    }

    public SectionModel[] Sections { get; set; } = [];
    
    public string PageUrl { get; set; }

    public string[] NavigationHistory => _navList.ToArray();

    public string PathName { get; init; }

    public bool CanSubmit => Sections.All(s => s.IsComplete);
    
    public void AddSection(SectionModel section)
    {
        section.PageUrl = $"/{PathName}/{section.PathName}";
        Sections = Sections.Concat([section]).ToArray();
    }
    
    public void AddNavigation(string url)
    {
        _navList.Add(url);
    }

    public void PopLastNavigationHistory()
    {
        if (_navList.Count > 0)
        {
            _navList.Remove(_navList.Last());
        }
    }

    public void PopAllNavigationHistory()
    {
        _navList.Clear();
    }
    
    public TPage FindPage<TPage>(string pageUrl) where TPage : PageModel
    {
        foreach (SectionModel section in Sections)
        {
            foreach (PageModel page in section.Pages)
            {
                if (pageUrl.EndsWith(page.PageUrl, StringComparison.Ordinal) && page is TPage modelPage)
                {
                    return modelPage;
                }
            }
        }

        throw new InvalidOperationException($"Unable to find page model for path '{pageUrl}'.");
    }

    public SectionModel FindSection(string sectionPageUrl)
    {
        SectionModel? section = Sections.FirstOrDefault(s => s.PageUrl == sectionPageUrl);
        
        return section ?? throw new InvalidOperationException($"Unable to find section model for path '{sectionPageUrl}'.");
    }
    
    public SectionModel FindSectionForPage(string pageUrl)
    {
        foreach (SectionModel section in Sections)
        {
            if (section.Pages.Any(page => page.PageUrl == pageUrl))
            {
                return section;
            }
        }

        throw new InvalidOperationException($"Unable to find section model for path '{pageUrl}'.");
    }

    public void Initialize()
    {
        _options ??= CreateOptions(this);

        // TODO: Could validate all paths are unique and throw exception if not
    }

    public static FormModel Deserialize(string json)
    {
        return JsonSerializer.Deserialize<FormModel>(json, _options)!;
    }
    
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, _options);
    }

    private static JsonSerializerOptions CreateOptions(FormModel form)
    {
        List<JsonDerivedType> derivedPageModelTypes = new();

        foreach (SectionModel section in form.Sections)
        {
            foreach (PageModel page in section.Pages)
            {
                if (derivedPageModelTypes.Any(t => 
                        t.TypeDiscriminator?.ToString() == page.GetType().Name))
                {
                    continue;
                }
                
                derivedPageModelTypes.Add(new JsonDerivedType(page.GetType(), page.GetType().Name));
            }
        }

        JsonSerializerOptions options = new()
        {
            TypeInfoResolver = new DefaultJsonTypeInfoResolver
            {
                Modifiers =
                {
                    typeInfo =>
                    {
                        if (typeInfo.Type == typeof(PageModel))
                        {
                            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
                            {
                                TypeDiscriminatorPropertyName = "$type"
                            };

                            foreach (JsonDerivedType type in derivedPageModelTypes)
                            {
                                typeInfo.PolymorphismOptions.DerivedTypes.Add(type);
                            }
                        }
                    }
                }
            }
        };

        return options;
    }
}