using System.Web;
using System.Web.Mvc;

namespace WebMVCLinhKienDienTu
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
