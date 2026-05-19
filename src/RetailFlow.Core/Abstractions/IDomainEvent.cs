using System;

namespace RetailFlow.Core.Abstractions
{
    /// <summary>
    /// Base marker interface for all domain events.
    /// Enables future CQRS / event sourcing integration.
    /// </summary>
    public interface IDomainEvent
    {
        DateTime OccurredAt { get; }
    }
}
