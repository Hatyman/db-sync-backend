using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MccSoft.DbSyncApp.App.Features.DbScheme.Enums;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class DbSchemeDto
{
    [Required]
    public Dictionary<string, TableSchemeDto> Tables { get; set; } = new();
    [Required]
    public Dictionary<string, SchemeDto> Schemas { get; set; } = new();
}
