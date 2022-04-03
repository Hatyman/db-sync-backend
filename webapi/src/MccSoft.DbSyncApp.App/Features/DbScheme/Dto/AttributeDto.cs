using MccSoft.WebApi.Patching.Models;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class AttributeDto
{
    public string Name { get; set; }

    public AttributeSchemeDto Scheme { get; set; }

    public bool IsUnique { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimary { get; set; }
    public bool IsForeignKey { get; set; }
}
