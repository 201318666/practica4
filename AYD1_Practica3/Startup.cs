using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AYD1_Practica3.Startup))]
namespace AYD1_Practica3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
