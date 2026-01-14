using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// Handler for queries.
/// </summary>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
