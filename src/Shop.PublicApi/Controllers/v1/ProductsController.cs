using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Product.Commands;
using Shop.Application.Product.Responses;
using Shop.PublicApi.Extensions;
using Shop.PublicApi.Models;
using Shop.Query.Application.Product.Queries;
using Shop.Query.QueriesModel;

namespace Shop.PublicApi.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
public class ProductsController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    ////////////////////////
    // POST: /api/products
    ////////////////////////

    /// <summary>
    /// Register a new product.
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">Returns the Id of the new product.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<CreatedProductResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody][Required] CreateProductCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    ///////////////////////
    // PUT: /api/products
    //////////////////////

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="command"></param>
    /// <response code="200">Returns the response with the success message.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no product is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpPut]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update([FromBody][Required] UpdateProductCommand command) =>
        (await _mediator.Send(command)).ToActionResult();

    //////////////////////////////
    // DELETE: /api/products/{id}
    //////////////////////////////

    /// <summary>
    /// Deletes the product by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Returns the response with the success message.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no product is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpDelete("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete([Required] Guid id) =>
        (await _mediator.Send(new DeleteProductCommand(id))).ToActionResult();

    ///////////////////////////
    // GET: /api/products/{id}
    ///////////////////////////

    /// <summary>
    /// Gets the product by Id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Returns the product.</response>
    /// <response code="400">Returns list of errors if the request is invalid.</response>
    /// <response code="404">When no product is found by the given Id.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet("{id:guid}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<ProductQueryModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById([Required] Guid id) =>
        (await _mediator.Send(new GetProductByIdQuery(id))).ToActionResult();

    //////////////////////
    // GET: /api/products
    //////////////////////

    /// <summary>
    /// Gets a list of all products.
    /// </summary>
    /// <response code="200">Returns the list of products.</response>
    /// <response code="500">When an unexpected internal error occurs on the server.</response>
    [HttpGet]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductQueryModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll() =>
        (await _mediator.Send(new GetAllProductQuery())).ToActionResult();
}