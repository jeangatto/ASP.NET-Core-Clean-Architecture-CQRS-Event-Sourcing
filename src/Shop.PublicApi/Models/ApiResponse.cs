using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Shop.PublicApi.Models;

/// <summary>
/// Classe responsável pela a padronização das respostas da API.
/// </summary>
public class ApiResponse
{
    #region Properties

    /// <summary>
    /// Indica se a requisição foi bem-sucedida.
    /// </summary>
    public bool Success { get; protected init; }

    /// <summary>
    /// Mensagem de sucesso.
    /// </summary>
    public string SuccessMessage { get; protected init; }

    /// <summary>
    /// O código do status HTTP.
    /// </summary>
    public int StatusCode { get; protected init; }

    /// <summary>
    /// Lista com os erros da requisição se não for bem-sucedida.
    /// </summary>
    public IEnumerable<ApiError> Errors { get; private init; } = Enumerable.Empty<ApiError>();

    #endregion

    #region HTTP Status 200 Ok

    /// <summary>
    /// Cria uma resposta com HTTP Status 200
    /// </summary>
    public static ApiResponse Ok()
        => new() { Success = true, StatusCode = StatusCodes.Status200OK };

    /// <summary>
    /// Cria uma resposta com HTTP Status 200
    /// </summary>
    /// <param name="successMessage">Mensagem de sucesso a ser exibida na resposta.</param>
    public static ApiResponse Ok(string successMessage)
        => new() { Success = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    #endregion

    #region HTTP Status 400 BadRequest

    /// <summary>
    /// Cria uma resposta com HTTP Status 400.
    /// </summary>
    public static ApiResponse BadRequest()
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest };

    /// <summary>
    /// Cria uma resposta com HTTP Status 400.
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro a ser exibida na resposta.</param>
    public static ApiResponse BadRequest(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateApiErrors(errorMessage) };

    /// <summary>
    /// Cria uma resposta com HTTP Status 400.
    /// </summary>
    /// <param name="errors">Lista de erros a serem exibidas na resposta.</param>
    public static ApiResponse BadRequest(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    #endregion

    #region HTTP Status 401 Unauthorized

    /// <summary>
    /// Cria uma resposta com HTTP Status 401.
    /// </summary>
    public static ApiResponse Unauthorized()
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized };

    /// <summary>
    /// Cria uma resposta com HTTP Status 401.
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro a ser exibida na resposta.</param>
    public static ApiResponse Unauthorized(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateApiErrors(errorMessage) };

    /// <summary>
    /// Cria uma resposta com HTTP Status 401.
    /// </summary>
    /// <param name="errors">Lista de erros a serem exibidas na resposta.</param>
    public static ApiResponse Unauthorized(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    #endregion

    #region HTTP Status 403 Forbidden

    /// <summary>
    /// Cria uma resposta com HTTP Status 403.
    /// </summary>
    public static ApiResponse Forbidden()
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden };

    /// <summary>
    /// Cria uma resposta com HTTP Status 403.
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro a ser exibida na resposta.</param>
    public static ApiResponse Forbidden(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateApiErrors(errorMessage) };

    /// <summary>
    /// Cria uma resposta com HTTP Status 403.
    /// </summary>
    /// <param name="errors">Lista de erros a serem exibidas na resposta.</param>
    public static ApiResponse Forbidden(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    #endregion

    #region HTTP Status 404 NotFound

    /// <summary>
    /// Cria uma resposta com HTTP Status 404.
    /// </summary>
    public static ApiResponse NotFound()
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound };

    /// <summary>
    /// Cria uma resposta com HTTP Status 404.
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro a ser exibida na resposta.</param>
    public static ApiResponse NotFound(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateApiErrors(errorMessage) };

    /// <summary>
    /// Cria uma resposta com HTTP Status 404.
    /// </summary>
    /// <param name="errors">Lista de erros a serem exibidas na resposta.</param>
    public static ApiResponse NotFound(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    #endregion

    #region HTTP Status 500 InternalServerError

    /// <summary>
    /// Cria uma resposta com HTTP Status 500.
    /// </summary>
    /// <param name="errorMessage">Mensagem de erro a ser exibida na resposta.</param>
    public static ApiResponse InternalServerError(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateApiErrors(errorMessage) };

    /// <summary>
    /// Cria uma resposta com HTTP Status 500.
    /// </summary>
    /// <param name="errors">Lista de erros a serem exibidas na resposta.</param>
    public static ApiResponse InternalServerError(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };

    #endregion

    private static IEnumerable<ApiError> CreateApiErrors(string errorMessage)
        => new[] { new ApiError(errorMessage) };
}