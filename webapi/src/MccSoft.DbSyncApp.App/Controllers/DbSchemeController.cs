using System.Collections.Generic;
using MccSoft.DbSyncApp.App.Features.DbScheme;
using MccSoft.DbSyncApp.App.Features.DbScheme.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MccSoft.DbSyncApp.App.Controllers;

[ApiController]
[Route("api/scheme")]
public class DbSchemeController
{
    [AllowAnonymous]
    [HttpGet]
    public DbSchemeDto GetTableScheme([FromServices] DbSchemeService dbSchemeService)
    {
        return dbSchemeService.GetScheme();
    }
}
