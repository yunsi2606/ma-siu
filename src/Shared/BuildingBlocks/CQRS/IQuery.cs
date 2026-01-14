using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// Marker interface for queries.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
