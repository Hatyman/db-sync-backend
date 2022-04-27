using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MccSoft.WebApi.Patching.Models;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class TableSchemeDto
{
    public string Name { get; set; } = "";
    public string EntityFullName { get; set; } = "";
    public string AssemblyName { get; set; } = "";
    [Required]
    public Dictionary<string, AttributeDto> Attributes { get; set; } = new();
    [Required]
    public List<string> PrimaryKeys { get; set; } = new();
    [NotRequired]
    public Dictionary<string, ConnectionSchemeDto> Connections { get; set; } = new();
}
