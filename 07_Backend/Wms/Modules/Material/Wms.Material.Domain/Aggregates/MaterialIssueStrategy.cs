using Volo.Abp.Domain.Entities.Auditing;

namespace Wms.Material.Domain.Aggregates;

public class MaterialIssueStrategy : AuditedAggregateRoot<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Strategy { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    private MaterialIssueStrategy() { }

    public MaterialIssueStrategy(Guid id, string code, string name, string strategy, string? description = null)
        : base(id)
    {
        Code = code;
        Name = name;
        Strategy = strategy;
        Description = description;
    }

    public void Update(string name, string strategy, string? description = null)
    {
        Name = name;
        Strategy = strategy;
        Description = description;
    }
}