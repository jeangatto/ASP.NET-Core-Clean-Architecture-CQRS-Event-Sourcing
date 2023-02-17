using Microsoft.AspNetCore.Http;

namespace Shop.PublicApi.Models;

/// <inheritdoc />
public sealed class ApiResponse<T> : ApiResponse
{
    /// <summary>
    /// Resultado da resposta.
    /// </summary>
    public T Result { get; private init; }

    /// <summary>
    /// Cria uma resposta com HTTP Status 200.
    /// </summary>
    /// <param name="result">Resultado da resposta.</param>
    public static ApiResponse<T> Ok(T result)
        => new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result };

    /// <summary>
    /// Cria uma resposta com HTTP Status 200.
    /// </summary>
    /// <param name="result">Resultado da resposta.</param>
    /// <param name="successMessage">Mensagem de sucesso a ser exibida na resposta.</param>
    public static ApiResponse<T> Ok(T result, string successMessage)
        => new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };
}