using System.Diagnostics.CodeAnalysis;
using MccSoft.DbSyncApp.App.Features.DbScheme.Enums;
using MccSoft.WebApi.Patching.Models;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class AttributeSchemeDto
{
    public RealmDataType? Type { get; set; }
    public DataFormat? Format { get; set; }
    [NotRequired]
    public string? Ref { get; set; }
}
