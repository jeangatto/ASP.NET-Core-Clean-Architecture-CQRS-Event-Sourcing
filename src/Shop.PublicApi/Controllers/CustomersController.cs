using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Customer.Commands;
using Shop.Application.Customer.Responses;
using Shop.PublicApi.Extensions;
using Shop.PublicApi.Models;
using Shop.Query.Application.Customer.Queries;
using Shop.Query.QueriesModel;

namespace Shop.PublicApi.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Register a new customer.
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">Returns the Id of the new client.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreatedCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody][Required] CreateCustomerCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    /// <summary>
    /// Updates an existing client.
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">Returns the response with the success message.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no client is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody][Required] UpdateCustomerCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    /// <summary>
    /// Deletes the client by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Returns the response with the success message.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no client is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpDelete("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([Required] Guid id) =>
        (await _mediator.Send(new DeleteCustomerCommand(id))).ToActionResult();

    /// <summary>
    /// Gets the client by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Returns the client.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no client is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CustomerQueryModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([Required] Guid id) =>
        (await _mediator.Send(new GetCustomerByIdQuery(id))).ToActionResult();

    /// <summary>
    /// Gets a list of all customers.
    /// </summary>
    /// <response code="200">Returns the list of clients.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerQueryModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll() =>
        (await _mediator.Send(new GetAllCustomerQuery())).ToActionResult();
}