using System.Collections.Generic;

namespace MccSoft.DbSyncApp.App.Features.DbScheme.Dto;

public class ConnectionSchemeDto
{
    public string TableName { get; set; } = "";
    public List<string> OwnAttributeNames { get; set; }
    public List<string> ExternalAttributeNames { get; set; }
    public bool isIncomingReference { get; set; }
}
