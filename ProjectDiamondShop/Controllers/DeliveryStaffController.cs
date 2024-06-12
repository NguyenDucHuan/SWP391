using DiamondShopBOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProjectDiamondShop.Controllers
{
    public class DeliveryStaffController : Controller
    {
        private readonly DiamondShopManagementEntities db = new DiamondShopManagementEntities(); // Entity Framework DbContext

        // GET: DeliveryStaff
        public ActionResult Index(string searchOrderId)
        {
            if (Session["RoleID"] == null || (int)Session["RoleID"] != 4)
            {
                return RedirectToAction("Index", "Home");
            }

            List<tblOrder> orders = GetOrders(searchOrderId);
            return View("DeliveryStaff", orders); // Ensure your view expects IEnumerable<tblOrder>
        }

        private List<tblOrder> GetOrders(string searchOrderId)
        {
            string deliveryStaffID = Session["UserID"].ToString();
            var ordersQuery = db.tblOrders.Where(o => o.deliveryStaffID == deliveryStaffID);

            if (!string.IsNullOrEmpty(searchOrderId))
            {
                ordersQuery = ordersQuery.Where(o => o.orderID.Contains(searchOrderId));
            }

            return ordersQuery.ToList();
        }
    }
}