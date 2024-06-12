using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using DiamondShopDAOs.CookieCartDAO;

namespace ProjectDiamondShop.Repositories
{
    public static class CartHelper
    {
        private static string GetCartCookieName(string userID)
        {
            return $"DiamondShopCart_{userID}";
        }

        public static CartDao GetCart(HttpContextBase context, string userID)
        {
            var cart = new CartDao();
            var cartCookieName = GetCartCookieName(userID);

            var cartCookie = context.Request.Cookies[cartCookieName];
            if (cartCookie != null && !string.IsNullOrEmpty(cartCookie.Value))
            {
                cart = JsonConvert.DeserializeObject<CartDao>(cartCookie.Value);
            }

            return cart;
        }

        public static void SaveCart(HttpContextBase context, string userID, CartDao cart)
        {
            var cartCookieName = GetCartCookieName(userID);
            var cartCookie = new HttpCookie(cartCookieName)
            {
                Value = JsonConvert.SerializeObject(cart),
                Expires = DateTime.Now.AddDays(7) // Cookie expires in 7 days
            };
            context.Response.Cookies.Add(cartCookie);
        }

        public static void AddToCart(HttpContextBase context, string userID, ItemCartDAO item)
        {
            var cart = GetCart(context, userID);
            var existingItem = cart.Items.FirstOrDefault(i => i.diamondID == item.diamondID);
            if (existingItem == null)
            {
                cart.Items.Add(item);
            }
            SaveCart(context, userID, cart);
        }

        public static void RemoveFromCart(HttpContextBase context, string userID, int diamondID)
        {
            var cart = GetCart(context, userID);
            var itemToRemove = cart.Items.FirstOrDefault(i => i.diamondID == diamondID);
            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
            }
            SaveCart(context, userID, cart);
        }

        public static void ClearCart(HttpContextBase context, string userID)
        {
            var cart = new CartDao();
            SaveCart(context, userID, cart);
        }
    }
}
