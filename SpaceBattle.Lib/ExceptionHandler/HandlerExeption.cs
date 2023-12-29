/* using Hwdtech;

namespace SpaceBattle.Lib
{
    public class HandlerException : IHandler
    {
        public ICommand SearchHandler(params Type[] args)
        {
            var _commandType = args[0];
            var _exceptionType = args[1];

            var keyCmd = _commandType.ToString();
            var keyException = _exceptionType.ToString();

            var tree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");

            IExceptionHandler? handler;

            if (tree.TryGetValue(keyCmd, out handler))
            {
                return (ICommand)handler;
            }

            else if (tree.TryGetValue(keyException, out handler))
            {
                return (ICommand)handler;
            }

            else if (tree.TryGetValue(keyCmd + keyException, out handler))
            {
                return (ICommand)handler;
            }
            else
            {
                //если нет обработчика для ошибок то получаем дефолтный для команды из айока
                var handler = (ICommand)tree.GetValueOrDefault(_exceptionType, IoC.Resolve<ICommand>("Commands.Handler", _commandType));
                return handler;
            }
        }
    }
} */

namespace SpaceBattle.Lib;
using Hwdtech;

public class HandlerExceptionStrategy : IStrategy
{
    public object Run(params object[] args)
    {
        IExceptionHandler? handler;

        var _commandType = args[0];
        var _exceptionType = args[1];

        var _keyCmd = _commandType.ToString();
        var _keyException = _exceptionType.ToString();

        var tree = IoC.Resolve<Dictionary<string, IExceptionHandler>>("ExceptionHandler.Tree");

        if (tree.TryGetValue(_keyCmd, out handler))
        {
            return handler;
        }

        else if(tree.TryGetValue(_keyException, out handler))
        {
            return handler;
        }

        else if(tree.TryGetValue(_keyCmd + _keyException, out handler))
        {
            return handler;
        }

        return IoC.Resolve<IExceptionHandler>("DefaultExceptionHandler");
    }
}
