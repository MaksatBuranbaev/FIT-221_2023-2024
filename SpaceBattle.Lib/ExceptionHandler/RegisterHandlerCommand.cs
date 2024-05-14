using Hwdtech;

namespace SpaceBattle.Lib;

public class RegisterHandlerCommand : ICommand
{
    private readonly Type[] _types;
    private readonly IExceptionHandler _exceptionHandler;

    public RegisterHandlerCommand(Type[] types, IExceptionHandler exceptionHandler)
    {
        _types = types;
        _exceptionHandler = exceptionHandler;
    }

    public void Execute()
    {
        var key = "";
        _types.ToList().ForEach(type =>
        {
            key += type.ToString();
        });
        var exceptionTree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");
        exceptionTree[key] = exceptionTree.GetValueOrDefault(key, _exceptionHandler);
    }
}
