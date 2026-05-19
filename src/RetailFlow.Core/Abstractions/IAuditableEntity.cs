using System;

namespace RetailFlow.Core.Abstractions
{
    /// <summary>
    /// Marker interface for entities that track creation and modification timestamps.
    /// Supports future audit logging and event sourcing patterns.
    /// </summary>
    public interface IAuditableEntity
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}
