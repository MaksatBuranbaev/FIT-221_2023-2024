namespace SpaceBattle.Lib;
using Scriban;

public class GetAdapterCode : IStrategy
{
    private Type? _type;
    public object Run(params object[] args)
    {
        _type = (Type)args[0];

        var templateString = @"public class {{ interface_name }}Adapter : {{ interface_name }}
{
    private IUObject _obj;
    public {{ interface_name }}Adapter(IUObject obj) => _obj = obj;{{for property in (properties)}}
    public {{property.property_type.name}} {{property.name}}
    {
    {{if property.can_read}}
    get
    {
        return ({{property.property_type.name}})_obj.GetProperty(""{{property.name}}"");
    }{{end}}
    {{if property.can_write}}
    set
    {
        _obj.SetProperty(""{{property.name}}"", _obj);
    }{{end}}
    }
{{end}}
}";

        var template = Template.Parse(templateString);
        var result = template.Render(new
        {
            interface_name = _type.Name,
            properties = _type.GetProperties().ToList()
        });
        return result;
    }
}
