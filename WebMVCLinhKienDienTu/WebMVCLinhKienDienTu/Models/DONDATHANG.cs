using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVCLinhKienDienTu.Models
{
    using System;
    using System.Collections.Generic;

    public partial class DONDATHANG
    {
        public int MaDonHang { get; set; }
        public Nullable<bool> Dathanhtoan { get; set; }
        public Nullable<bool> Tinhtranggiaohang { get; set; }
        public Nullable<System.DateTime> Ngaydat { get; set; }
        public Nullable<System.DateTime> Ngaygiao { get; set; }
        public string Id { get; set; }

        public virtual AspNetUser AspNetUser { get; set; }
    }
}
