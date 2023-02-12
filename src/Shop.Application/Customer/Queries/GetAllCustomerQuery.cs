using System.Collections.Generic;
using Ardalis.Result;
using MediatR;
using Shop.Domain.QueriesModel;

namespace Shop.Application.Customer.Queries;

public class GetAllCustomerQuery : IRequest<Result<IEnumerable<CustomerQueryModel>>>
{
}