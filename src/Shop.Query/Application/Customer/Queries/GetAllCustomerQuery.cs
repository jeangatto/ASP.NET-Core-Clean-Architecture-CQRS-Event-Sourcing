using System.Collections.Generic;
using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Customer.Queries;

public class GetAllCustomerQuery : IRequest<Result<IEnumerable<CustomerQueryModel>>>
{
}
