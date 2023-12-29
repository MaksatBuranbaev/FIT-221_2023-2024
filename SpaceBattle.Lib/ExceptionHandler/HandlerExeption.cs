namespace SpaceBattle.Lib;
using Hwdtech;

public class HandlerExceptionStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var _keyCmd = args[0].ToString();
        var _keyException = args[1].ToString();

        var tree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");

        if (tree.ContainsKey(_keyCmd))
        {
            var handlerCmd = tree[_keyCmd];
            return handlerCmd;
        }

        else if (tree.ContainsKey(_keyException))
        {
            var handlerExc = tree[_keyException];
            return handlerExc;
        }

        else
        {
            return IoC.Resolve<IExceptionHandler>("DefaultExceptionHandler");
        }
    }
}
