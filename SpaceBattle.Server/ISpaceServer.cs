using System.Net;
using CoreWCF;
using CoreWCF.OpenApi.Attributes;
using CoreWCF.Web;

namespace SpaceBattle.Server
{
    [ServiceContract]
    [OpenApiBasePath("/spacebattle")]
    internal interface ISpaceServer
    {
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/order")]
        [OpenApiTag("spacebattle")]
        [OpenApiResponse(ContentTypes = new[] { "application/json" }, Description = "Success", StatusCode = HttpStatusCode.OK, Type = typeof(GameContract))]
        string Order(
            [OpenApiParameter(ContentTypes = new[] { "application/json" }, Description = "param description.")] GameContract param);
    }
}
