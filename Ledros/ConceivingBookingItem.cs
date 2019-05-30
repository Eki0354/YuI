using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ledros
{
    #region ConceivingBookingItem

    /// <summary> 仅用于获取订单时使用的临时数据类型 </summary>
    public class ConceivingBookingItem
    {
        public long UID { get; set; }
        public long ID { get; set; }
        public string Channel { get; set; }
        public string FullName { get; set; }
        public string ResNumber { get; set; }
    }

    #endregion
}
