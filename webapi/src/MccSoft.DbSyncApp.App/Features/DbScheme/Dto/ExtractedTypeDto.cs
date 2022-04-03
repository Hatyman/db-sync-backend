using System;
using System.Collections.Generic;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class ExtractedTypeDto
{
    public AttributeSchemeDto Scheme { get; set; }
    public SchemeDto? Details { get; set; }
}
