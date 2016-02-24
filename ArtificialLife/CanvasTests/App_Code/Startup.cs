using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CanvasTests.Startup))]
namespace CanvasTests
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
