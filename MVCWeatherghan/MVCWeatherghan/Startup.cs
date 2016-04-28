using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCWeatherghan.Startup))]
namespace MVCWeatherghan
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
