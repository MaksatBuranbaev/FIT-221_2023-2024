using CoreWCF;

namespace WebHttp
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    internal class WebApi : IWebApi
    {
        public GameContract BodyEcho(GameContract param) => param;
    }
}
