using DiamondShopDAOs.CookieCartDAO;
using ProjectDiamondShop.Models;
using ProjectDiamondShop.Repositories;
using System.Web.Mvc;

public class CartController : Controller
{
    private bool IsAdmin()
    {
        return Session["RoleID"] != null && (int)Session["RoleID"] == 2;
    }
    private bool IsSaleStaff()
    {
        return Session["RoleID"] != null && (int)Session["RoleID"] == 5;
    }
    private bool IsDelivery()
    {
        return Session["RoleID"] != null && (int)Session["RoleID"] == 4;
    }
    private bool IsManager()
    {
        return Session["RoleID"] != null && (int)Session["RoleID"] == 3;
    }
    private string GetUserID()
    {
        if (Session["UserID"] == null)
        {
            Session["ReturnUrl"] = Url.Action("Index", "Cart");
            return null;
        }
        return Session["UserID"].ToString();
    }

    public ActionResult Index()
    {
        if (IsAdmin())
        {
            return RedirectToAction("Index", "Manager");
        }
        if (IsManager())
        {
            return RedirectToAction("Index", "Manager");
        }
        if (IsSaleStaff())
        {
            return RedirectToAction("Index", "SaleStaff");
        }
        if (IsDelivery())
        {
            return RedirectToAction("Index", "DeliveryStaff");
        }
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Account");
        }
        var cart = CartHelper.GetCart(HttpContext, userID);
        return View(cart);
    }

    [HttpPost]
    public ActionResult AddToCart(int settingID, int accentStoneID, int diamondID, int? settingSize)
    {
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Account");
        }
        var cartItem = new ItemCartDAO(settingID, accentStoneID, diamondID, settingSize);
        CartHelper.AddToCart(HttpContext, userID, cartItem);
        return RedirectToAction("Index", "Diamonds");
    }


    [HttpPost]
    public ActionResult RemoveFromCart(int diamondID)
    {
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Account");
        }
        CartHelper.RemoveFromCart(HttpContext, userID, diamondID);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public ActionResult ClearCart()
    {
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Account");
        }
        CartHelper.ClearCart(HttpContext, userID);
        return RedirectToAction("Index");
    }
}
