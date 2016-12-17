using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebMVCLinhKienDienTu.Startup))]
namespace WebMVCLinhKienDienTu
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
