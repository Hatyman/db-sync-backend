using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class SchemeDto
{
    public string Name { get; set; }
    public bool? AreValuesConfigured { get; set; }
    public Dictionary<string, int>? Enum { get; set; }
}
