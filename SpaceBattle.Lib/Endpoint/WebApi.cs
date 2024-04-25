using CoreWCF;
using SpaceBattle.Lib;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public GameContract BodyEcho(GameContract param) 
        {
            var endpoint = new Endpoint(param);
            endpoint.Execute();
            return param;
        }
    }
}
