using System.Collections.Generic;
using Ardalis.Result;
using MediatR;
using Shop.Query.QueriesModel;

namespace Shop.Query.Application.Product.Queries;

public class GetAllProductQuery : IRequest<Result<IEnumerable<ProductQueryModel>>>;