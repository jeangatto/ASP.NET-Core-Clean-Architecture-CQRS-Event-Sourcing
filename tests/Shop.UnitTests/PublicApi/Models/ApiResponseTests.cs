using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Shop.PublicApi.Models;
using Xunit;
using Xunit.Categories;

namespace Shop.UnitTests.PublicApi.Models;

[UnitTest]
public class ApiResponseTests
{
    [Fact]
    public void FactoryMethods_ShouldSetStatusAndErrors()
    {
        var error = new ApiErrorResponse("error");
        var errors = new[] { error };

        ApiResponse.Ok().Should().Match<ApiResponse>(response =>
            response.Success && response.StatusCode == StatusCodes.Status200OK);
        ApiResponse.Ok("saved").SuccessMessage.Should().Be("saved");

        ApiResponse.BadRequest().StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        ApiResponse.BadRequest("invalid").Errors.Should().ContainSingle().Which.Message.Should().Be("invalid");
        ApiResponse.BadRequest(errors).Errors.Should().BeEquivalentTo(errors);

        ApiResponse.Unauthorized().StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        ApiResponse.Unauthorized("unauthorized").Errors.Should().ContainSingle();
        ApiResponse.Unauthorized(errors).Errors.Should().BeEquivalentTo(errors);

        ApiResponse.Forbidden().StatusCode.Should().Be(StatusCodes.Status403Forbidden);
        ApiResponse.Forbidden("forbidden").Errors.Should().ContainSingle();
        ApiResponse.Forbidden(errors).Errors.Should().BeEquivalentTo(errors);

        ApiResponse.NotFound().StatusCode.Should().Be(StatusCodes.Status404NotFound);
        ApiResponse.NotFound("missing").Errors.Should().ContainSingle();
        ApiResponse.NotFound(errors).Errors.Should().BeEquivalentTo(errors);

        ApiResponse.InternalServerError("failure").Errors.Should().ContainSingle();
        ApiResponse.InternalServerError(errors).StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }

    [Fact]
    public void ToString_ShouldReportWhetherErrorsExist()
    {
        ApiResponse.Ok().ToString().Should().Be("Success: True | StatusCode: 200 | HasErrors: False");
        ApiResponse.BadRequest([new("invalid")])
            .ToString().Should().Be("Success: False | StatusCode: 400 | HasErrors: True");
    }
}
