using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shop.PublicApi.Models;

public class ApiResponse
{
    [JsonConstructor]
    public ApiResponse(bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
    {
        Success = success;
        SuccessMessage = successMessage;
        StatusCode = statusCode;
        Errors = errors;
    }

    public ApiResponse()
    {
    }

    public bool Success { get; protected set; }
    public string SuccessMessage { get; protected set; }
    public int StatusCode { get; protected set; }
    public IEnumerable<ApiErrorResponse> Errors { get; private init; } = Enumerable.Empty<ApiErrorResponse>();

    public static ApiResponse Ok() =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK };

    public static ApiResponse Ok(string successMessage) =>
        new() { Success = true, StatusCode = StatusCodes.Status200OK, SuccessMessage = successMessage };

    public static ApiResponse BadRequest() =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest };

    public static ApiResponse BadRequest(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = CreateErrors(errorMessage) };

    public static ApiResponse BadRequest(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status400BadRequest, Errors = errors };

    public static ApiResponse Unauthorized() =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized };

    public static ApiResponse Unauthorized(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status401Unauthorized, Errors = errors };

    public static ApiResponse Forbidden() =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden };

    public static ApiResponse Forbidden(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = CreateErrors(errorMessage) };

    public static ApiResponse Forbidden(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status403Forbidden, Errors = errors };

    public static ApiResponse NotFound() =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound };

    public static ApiResponse NotFound(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = CreateErrors(errorMessage) };

    public static ApiResponse NotFound(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status404NotFound, Errors = errors };

    public static ApiResponse InternalServerError(string errorMessage) =>
        new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = CreateErrors(errorMessage) };

    public static ApiResponse InternalServerError(IEnumerable<ApiErrorResponse> errors) =>
        new() { Success = false, StatusCode = StatusCodes.Status500InternalServerError, Errors = errors };

    private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
        new[] { new ApiErrorResponse(errorMessage) };

    public override string ToString() =>
        $"Success: {Success} | StatusCode: {StatusCode} | HasErrors: {Errors.Any()}";
}