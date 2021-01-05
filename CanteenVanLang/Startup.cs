using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CanteenVanLang.Startup))]
namespace CanteenVanLang
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
