using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Renaissance.Core.Startup))]
namespace Renaissance.Core
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
