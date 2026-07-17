using Volo.Abp.Application.Dtos;

namespace Wms.Material.Application.Contracts.Dtos;

public class MaterialIssueStrategyOutputDto : EntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class MaterialIssueStrategyCreateDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class MaterialIssueStrategyUpdateDto
{
    public string Name { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class MaterialIssueStrategyQueryDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}