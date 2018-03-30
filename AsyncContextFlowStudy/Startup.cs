using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AsyncContextFlowStudy.Startup))]
namespace AsyncContextFlowStudy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
