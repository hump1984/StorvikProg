using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StorvikProg.Startup))]
namespace StorvikProg
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
