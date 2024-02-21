namespace SpaceBattle.Lib;
using Hwdtech;

public class HandlerExceptionStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        var _keyCmd = args[0].ToString();
        var _keyException = args[1].ToString();

        var tree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");

        if (tree.ContainsKey(_keyCmd + _keyException))
        {
            var handlerCmdExc = tree[_keyCmd + _keyException];
            return handlerCmdExc;
        }

        else
        {
            return IoC.Resolve<IExceptionHandler>("DefaultExceptionHandler");
        }
    }
}
