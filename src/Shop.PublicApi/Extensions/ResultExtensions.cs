using System.Linq;
using Ardalis.Result;
using Microsoft.AspNetCore.Mvc;
using Shop.PublicApi.Models.Responses;

namespace Shop.PublicApi.Extensions;

internal static class ResultExtensions
{
    /// <summary>
    /// Converts a custom Result object to an IActionResult.
    /// </summary>
    /// <param name="result">The Result object to convert.</param>
    /// <returns>An IActionResult representing the Result object.</returns>
    public static IActionResult ToActionResult(this Result result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse.Ok(result.SuccessMessage))
            : result.ToHttpNonSuccessResult();

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IActionResult"/>.
    /// </summary>
    /// <typeparam name="T">The type of the result value.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result.</returns>
    public static IActionResult ToActionResult<T>(this Result<T> result) =>
        result.IsSuccess
            ? new OkObjectResult(ApiResponse<T>.Ok(result.Value, result.SuccessMessage))
            : result.ToHttpNonSuccessResult();

    private static IActionResult ToHttpNonSuccessResult(this IResult result)
    {
        var errors = result.Errors.Select(error => new ApiErrorResponse(error)).ToList();

        switch (result.Status)
        {
            case ResultStatus.Invalid:

                var validationErrors = result
                    .ValidationErrors
                    .ConvertAll(validation => new ApiErrorResponse(validation.ErrorMessage));

                return new BadRequestObjectResult(ApiResponse.BadRequest(validationErrors));

            case ResultStatus.NotFound:
                return new NotFoundObjectResult(ApiResponse.NotFound(errors));

            case ResultStatus.Unauthorized:
                return new UnauthorizedObjectResult(ApiResponse.Unauthorized(errors));

            default:
                return new BadRequestObjectResult(ApiResponse.BadRequest(errors));
        }
    }
}