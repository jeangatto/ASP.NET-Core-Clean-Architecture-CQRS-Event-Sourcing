using System;
using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using MediatR;
using Shop.Application.Product.Responses;

namespace Shop.Application.Product.Commands;

public class CreateProductCommand : IRequest<Result<CreatedProductResponse>>
{
    [Required]
    [MaxLength(100)]
    [DataType(DataType.Text)]
    public string Name { get; set; }

    [Required]
    [MaxLength(500)]
    [DataType(DataType.MultilineText)]
    public string Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
}
