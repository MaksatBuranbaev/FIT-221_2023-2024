namespace SpaceBattle.Lib;
using Hwdtech;

public class HandlerExceptionStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        IExceptionHandler handler;

        var _keyCmd = args[0].ToString();
        var _keyException = args[1].ToString();

        var tree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");

        if (tree.TryGetValue(_keyCmd, out handler))
        {
            return handler;
        }

        else if(tree.TryGetValue(_keyException, out handler))
        {
           return handler;
        }

        else
        {
            return IoC.Resolve<IExceptionHandler>("DefaultExceptionHandler");
        }
    }
}
