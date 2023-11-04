using System.Text.Json.Serialization;

namespace Shop.PublicApi.Models;

public sealed class ApiErrorResponse
{
    [JsonConstructor]
    public ApiErrorResponse(string message) => Message = message;

    public string Message { get; }

    public override string ToString() => Message;
}