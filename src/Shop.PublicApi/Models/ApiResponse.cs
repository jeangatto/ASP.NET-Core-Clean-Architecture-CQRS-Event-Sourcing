using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Shop.PublicApi.Models;

public class ApiResponse
{
    #region Properties

    public bool Success { get; protected init; }
    public string SuccessMessage { get; protected init; }
    public int StatusCode { get; protected init; }
    public IEnumerable<ApiError> Errors { get; private init; } = Enumerable.Empty<ApiError>();

    #endregion

    #region HTTP Status 200 Ok

    public static ApiResponse Ok()
        => new() { Success = true, StatusCode = StatusCodes.Status200OK };

    public static ApiResponse Ok(string successMessage)
        => new() { Success = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    #endregion

    #region HTTP Status 400 BadRequest

    public static ApiResponse BadRequest()
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse BadRequest(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateApiErrors(errorMessage) };

    public static ApiResponse BadRequest(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    #endregion

    #region HTTP Status 401 Unauthorized

    public static ApiResponse Unauthorized()
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse Unauthorized(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateApiErrors(errorMessage) };

    public static ApiResponse Unauthorized(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    #endregion

    #region HTTP Status 403 Forbidden

    public static ApiResponse Forbidden()
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse Forbidden(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateApiErrors(errorMessage) };

    public static ApiResponse Forbidden(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    #endregion

    #region HTTP Status 404 NotFound

    public static ApiResponse NotFound()
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse NotFound(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateApiErrors(errorMessage) };

    public static ApiResponse NotFound(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    #endregion

    #region HTTP Status 500 InternalServerError

    public static ApiResponse InternalServerError(string errorMessage)
        => new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateApiErrors(errorMessage) };

    public static ApiResponse InternalServerError(IEnumerable<ApiError> errors)
        => new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };
    #endregion

    private static IEnumerable<ApiError> CreateApiErrors(string errorMessage)
        => new[] { new ApiError(errorMessage) };
}