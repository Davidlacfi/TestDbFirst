using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestDbFirst.Startup))]
namespace TestDbFirst
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
