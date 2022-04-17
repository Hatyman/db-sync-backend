using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MccSoft.DbSyncApp.App.Features.Hub;

public class TransactionHub : Microsoft.AspNetCore.SignalR.Hub
{
    public async Task<string> Send(string message)
    {
        await Clients.All.SendAsync("test", message);

        return $"{message} Processed";
    }
}
