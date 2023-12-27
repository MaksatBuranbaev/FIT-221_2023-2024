using Hwdtech;

namespace SpaceBattle.Lib;

public class RegisterHandlerCommand : ICommand
{
    private readonly Type _cmdType;
    private readonly Type _exceptionType;
    private readonly IExceptionHandler _exceptionHandler;

    public RegisterHandlerCommand(Type cmdType, Type exceptionType, IExceptionHandler exceptionHandler)
    {
        _cmdType = cmdType;
        _exceptionType = exceptionType;
        _exceptionHandler = exceptionHandler;
    }

    public void Execute()
    {
        var exceptionTree = IoC.Resolve<Dictionary<Type, object>>("ExceptionHandler.Tree");
        exceptionTree[_cmdType] = exceptionTree.GetValueOrDefault(_cmdType, new Dictionary<Type, object>());

        var node = (Dictionary<Type, object>)exceptionTree[_cmdType];
        node[_exceptionType] = node.GetValueOrDefault(_exceptionType, _exceptionHandler);
    }
}
