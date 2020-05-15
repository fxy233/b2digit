using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Projet_pilate.Startup))]
namespace Projet_pilate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
