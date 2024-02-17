using System;
using System.ComponentModel.DataAnnotations;
using Ardalis.Result;
using MediatR;

namespace Shop.Application.Product.Commands;

public class UpdateProductCommand : IRequest<Result>
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    public decimal Price { get; set; }
}