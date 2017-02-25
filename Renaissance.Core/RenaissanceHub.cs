using Microsoft.AspNet.SignalR;

namespace Renaissance.Core
{
    public class RenaissanceHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}