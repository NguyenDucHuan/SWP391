using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondShopBOs
{
    public class OrderViewModel
    {
        public List<tblOrder> currentOrders { get; set; }
        public List<tblOrder> historyOrder { get; set; }

        public OrderViewModel()
        {
            currentOrders = new List<tblOrder>();
            historyOrder = new List<tblOrder>();
        }
        public OrderViewModel(List<tblOrder> currentOrders, List<tblOrder> historyOrder)
        {
            this.currentOrders = currentOrders;
            this.historyOrder = historyOrder;
        }
    }
}
