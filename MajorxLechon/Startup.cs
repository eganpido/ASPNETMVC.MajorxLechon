using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MajorxLechon.Startup))]
namespace MajorxLechon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
