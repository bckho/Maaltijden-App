using MaaltijdenApp_WebApp.Models;
using MaaltijdenApp_WebApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Infrastructure
{
    public class CartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            // Check if state is Create or Update
            var initMP = HttpContext.Session.GetJson<InitializeUpdateMealPackage>("InitializeMealPackage");
            var initUpdateMP = HttpContext.Session.GetJson<InitializeUpdateMealPackage>("InitializeUpdateMealPackage");

            // Check checkout type
            if (initMP != null) ViewBag.Checkout = "Create";
            if (initUpdateMP != null) ViewBag.Checkout = "Update";

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();
            CartViewModel cartVM = new CartViewModel
            {
                CartItems = cart
            };

            return View(cartVM);
        }
    }
}
