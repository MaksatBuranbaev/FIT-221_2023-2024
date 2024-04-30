using CoreWCF;

namespace SpaceBattle.Server
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
