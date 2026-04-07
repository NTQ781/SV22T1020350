using SV22T1020350.Models.Sales;
using System.Collections.Generic;

namespace SV22T1020350.Admin.Models
{
    public class OrderDetailModel
    {
        public OrderViewInfo? Order { get; set; }
        public List<OrderDetailViewInfo> Details { get; set; } = new List<OrderDetailViewInfo>();
    }
}