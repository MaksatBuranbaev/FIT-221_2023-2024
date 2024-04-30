using CoreWCF;
using SpaceBattle.Lib;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class SpaceServer : ISpaceServer
    {
        public GameContract Order(GameContract param) 
        {
            var endpoint = new Endpoint(param);
            endpoint.Execute();
            return param;
        }
    }
}
