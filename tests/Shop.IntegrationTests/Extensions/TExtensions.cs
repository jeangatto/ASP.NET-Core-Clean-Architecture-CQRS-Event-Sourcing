using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Shop.Core.Extensions;

namespace Shop.IntegrationTests.Extensions;

public static class TExtensions
{
    public static HttpContent ToJsonHttpContent<TRequest>(this TRequest request) =>
        new StringContent(request.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json);
}
