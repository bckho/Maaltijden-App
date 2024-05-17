using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using MaaltijdenApp_WebApp.Infrastructure;
using MaaltijdenApp_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Controllers
{
    [Authorize]
    [Authorize(Roles = "employee")]
    public class CartController : Controller
    {
        private IProductRepository _productRepository;

        /// <summary>
        /// This controller manages the cart when creating a Meal Package.
        /// </summary>
        /// <param name="productRepository">Inferface param for Product</param>
        public CartController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        /// <summary>
        /// AddProduct adds a product by the given productId.
        /// </summary>
        /// <param name="productId">Guid of product</param>
        /// <returns>View</returns>
        public async Task<IActionResult> AddProduct(string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                return RedirectToAction("NotFound", "Home");
            }

            // Check if product exists
            var productId = Guid.Parse(id);
            var product = await _productRepository.GetById(productId);

            if (product == null)
            {
                ModelState.AddModelError("", "Er zijn technische problemen, probeer het later opnieuw.");
                return RedirectToAction("SelectProduct", "MealPackage");
            }

            // Get current Cart, otherwise create new List with CartItems
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            var cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            // Check if product is already added to Cart.
            if (cartItem == null)
            {
                cart.Add(new CartItem(product));
            }

            // Set Cart json
            HttpContext.Session.SetJson("Cart", cart);

            TempData["Success"] = $"Product '{product.Name}' is toegevoegd!";

            return RedirectToAction("SelectProduct", "MealPackage");
        }

        /// <summary>
        /// Remove product from cart.
        /// </summary>
        /// <param name="id">Product ID</param>
        /// <returns>Redirect view</returns>
        public IActionResult RemoveProduct(string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                return RedirectToAction("SelectProduct", "MealPackage");
            }

            var productId = Guid.Parse(id);

            // Get current Cart, otherwise create new List with CartItems
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Get CartItem from cart
            var cartItem = cart.FirstOrDefault(c => c.ProductId.Equals(productId));

            if (cartItem != null)
            {
                // Remove product from cart
                cart.Remove(cartItem);
            }

            // Set Cart json
            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("SelectProduct", "MealPackage");
        }

        /// <summary>
        /// Checkout assembled MealPackage.
        /// </summary>
        /// <returns>Redirect View</returns>
        public IActionResult Checkout()
        {
            // Get current Cart
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Get InitializeMealPackage
            var mp = HttpContext.Session.GetJson<InitializeMealPackage>("InitializeMealPackage");

            // Check if cart and InitializeMealPackage are valid.
            if (cart.Any() && mp != null)
            {
                return RedirectToAction("Create", "MealPackage");
            }

            return RedirectToAction("SelectProduct", "MealPackage");
        }

        /// <summary>
        /// Checkout updated MealPackage
        /// </summary>
        /// <returns>Redirect View</returns>
        public IActionResult CheckoutUpdate()
        {
            // Get current Cart
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            // Get InitializeUpdateMealPackage
            var mp = HttpContext.Session.GetJson<InitializeUpdateMealPackage>("InitializeUpdateMealPackage");

            // Check if cart and InitializeMealPackage are valid.
            if (cart.Any() && mp != null)
            {
                return RedirectToAction("Update", "MealPackage");
            }

            return RedirectToAction("SelectProduct", "MealPackage");
        }
    }
}
