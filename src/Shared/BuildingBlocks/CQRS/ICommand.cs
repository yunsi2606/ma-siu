using MediatR;

namespace BuildingBlocks.CQRS;

/// <summary>
/// Marker interface for commands that return a result.
/// </summary>
/// <typeparam name="TResponse">Response type</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

/// <summary>
/// Marker interface for commands with no return value.
/// </summary>
public interface ICommand : IRequest<Unit>
{
}
