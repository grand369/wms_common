using Wms.Shared.Domain.Enums;

namespace Wms.Inventory.Domain.StateMachines;

/// <summary>
/// Inventory Status State Machine (SM-04) — defines valid status transition rules.
/// Available → Frozen/Reserved/QualityHold
/// Reserved → Available
/// Frozen → Available
/// QualityHold → Available/Scrapped
/// </summary>
public class InventoryStatusStateMachine
{
    private static readonly Dictionary<InventoryStatus, List<InventoryStatus>> _transitions = new()
    {
        { InventoryStatus.Available, new List<InventoryStatus>
            { InventoryStatus.Frozen, InventoryStatus.Reserved, InventoryStatus.QualityHold } },
        { InventoryStatus.Reserved, new List<InventoryStatus>
            { InventoryStatus.Available } },
        { InventoryStatus.Frozen, new List<InventoryStatus>
            { InventoryStatus.Available } },
        { InventoryStatus.InTransit, new List<InventoryStatus>
            { InventoryStatus.Available } },
        { InventoryStatus.QualityHold, new List<InventoryStatus>
            { InventoryStatus.Available, InventoryStatus.Scrapped } },
        { InventoryStatus.Scrapped, new List<InventoryStatus>() } // Terminal state
    };

    /// <summary>Check if a transition from current to next status is valid.</summary>
    public bool CanTransition(InventoryStatus currentStatus, InventoryStatus newStatus)
    {
        if (currentStatus == newStatus)
        {
            return true; // Same status is always valid (no-op)
        }

        if (!_transitions.TryGetValue(currentStatus, out var allowedTargets))
        {
            return false;
        }

        return allowedTargets.Contains(newStatus);
    }

    /// <summary>Get all valid target statuses from a given current status.</summary>
    public List<InventoryStatus> GetValidTransitions(InventoryStatus currentStatus)
    {
        return _transitions.TryGetValue(currentStatus, out var targets) ? targets : new List<InventoryStatus>();
    }
}
