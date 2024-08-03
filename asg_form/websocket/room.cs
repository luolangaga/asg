

using Microsoft.AspNetCore.SignalR;
namespace asg_form.Controllers.Hubs;

public class room : Hub
{
    public async Task login(string message)
    {
        await Clients.All.SendAsync("havanewxb", message);
    }
   

}
