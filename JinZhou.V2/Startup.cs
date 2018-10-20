using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JinZhou.V2.Startup))]
namespace JinZhou.V2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
