using System.Diagnostics.CodeAnalysis;
using CoreWCF;
using Hwdtech;

namespace SpaceBattle.Server
{
    [ExcludeFromCodeCoverage]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class SpaceServer : ISpaceServer
    {
        public GameContract Order(GameContract param)
        {
            IoC.Resolve<Lib.ICommand>("Endpoint", param).Execute();
            return param;
        }
    }
}
