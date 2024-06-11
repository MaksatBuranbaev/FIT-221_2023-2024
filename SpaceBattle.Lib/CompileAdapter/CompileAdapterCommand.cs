namespace SpaceBattle.Lib;
using System.Reflection;
using Hwdtech;

public class CompileAdapterCommand : ICommand
{
    private readonly Type _objectType;
    private readonly Type _targetType;

    public CompileAdapterCommand(Type objectType, Type targetType)
    {
        _objectType = objectType;
        _targetType = targetType;
    }

    public void Execute()
    {
        var code = IoC.Resolve<string>("Adapter.Code", _objectType, _targetType);
        var assembly = IoC.Resolve<Assembly>("Code.Compile", code);

        var map = IoC.Resolve<IDictionary<KeyValuePair<Type, Type>, Assembly>>("Adapter.Map");
        var pair = new KeyValuePair<Type, Type>(_objectType, _targetType);
        map[pair] = assembly;
    }
}
