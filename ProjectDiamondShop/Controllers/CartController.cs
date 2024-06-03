using ProjectDiamondShop.Models;
using ProjectDiamondShop.Repositories;
using System.Web.Mvc;

public class CartController : Controller
{
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
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Login");
        }
        var cart = CartHelper.GetCart(HttpContext, userID);
        return View(cart);
    }

    [HttpPost]
    public ActionResult AddToCart(int diamondID, string diamondName, decimal diamondPrice)
    {
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Login");
        }
        var cartItem = new CartItem
        {
            DiamondID = diamondID,
            DiamondName = diamondName,
            DiamondPrice = diamondPrice
        };
        CartHelper.AddToCart(HttpContext, userID, cartItem);
        return RedirectToAction("Index", "Diamonds");
    }


    [HttpPost]
    public ActionResult RemoveFromCart(int diamondID)
    {
        var userID = GetUserID();
        if (string.IsNullOrEmpty(userID))
        {
            return RedirectToAction("Index", "Login");
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
            return RedirectToAction("Index", "Login");
        }
        CartHelper.ClearCart(HttpContext, userID);
        return RedirectToAction("Index");
    }
}
