using System;
using System.Collections.Generic;
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

    public CustomersController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Cadastra um novo cliente.
    /// </summary>
    /// <param name="command">A requisição.</param>
    /// <response code="200">Retorna o Id do novo cliente.</response>
    /// <response code="400">Retorna lista de erros, se a requisição for inválida.</response>
    /// <response code="500">Quando ocorre um erro interno inesperado no servidor.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreatedCustomerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
        => (await _mediator.Send(command)).ToActionResult();

    /// <summary>
    /// Atualiza um cliente existente.
    /// </summary>
    /// <param name="command">A requisição.</param>
    /// <response code="200">Retorna a resposta com a mensagem de sucesso.</response>
    /// <response code="400">Retorna lista de erros, se a requisição for inválida.</response>
    /// <response code="404">Quando nenhum cliente é encontrado pelo Id fornecido.</response>
    /// <response code="500">Quando ocorre um erro interno inesperado no servidor.</response>
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody] UpdateCustomerCommand command)
        => (await _mediator.Send(command)).ToActionResult();

    /// <summary>
    /// Deleta o cliente pelo Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <response code="200">Retorna a resposta com a mensagem de sucesso.</response>
    /// <response code="400">Retorna lista de erros, se a requisição for inválida.</response>
    /// <response code="404">Quando nenhum cliente é encontrado pelo Id fornecido.</response>
    /// <response code="500">Quando ocorre um erro interno inesperado no servidor.</response>
    [HttpDelete("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(Guid id)
        => (await _mediator.Send(new DeleteCustomerCommand(id))).ToActionResult();

    /// <summary>
    /// Obtém o cliente pelo Id.
    /// </summary>
    /// <param name="id">O Id.</param>
    /// <response code="200">Retorna o cliente.</response>
    /// <response code="400">Retorna lista de erros, se a requisição for inválida.</response>
    /// <response code="404">Quando nenhum cliente é encontrado pelo Id fornecido.</response>
    /// <response code="500">Quando ocorre um erro interno inesperado no servidor.</response>
    [HttpGet("{id}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CustomerQueryModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(Guid id)
        => (await _mediator.Send(new GetCustomerByIdQuery(id))).ToActionResult();

    /// <summary>
    /// Obtém uma lista com todos os clientes.
    /// </summary>
    /// <response code="200">Retorna a lista de clientes.</response>
    /// <response code="500">Quando ocorre um erro interno inesperado no servidor.</response>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerQueryModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
        => (await _mediator.Send(new GetAllCustomerQuery())).ToActionResult();
}