using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using MaaltijdenApp_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly IProductRepository _productRepository;

        public ProductController(
            UserManager<IdentityUser> userMgr,
            IProductRepository productRepository)
        {
            _userMgr = userMgr;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            try
            {
                var products = _productRepository.GetAll();
                return View(products);
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Home");
            }
        }

        [Authorize(Roles = "employee")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> Create(ProductCreate productCreate)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Voer geldige gegevens in.");
                return View(productCreate);
            }

            // Initialize new product
            var product = new Product();

            // Add image if uploaded
            if (productCreate.Image != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                await productCreate.Image.CopyToAsync(memoryStream);

                product.Image = memoryStream.ToArray();
            }

            // Assign attributes to new product
            product.Id = Guid.NewGuid();
            product.Name = productCreate.Name;
            product.Description = productCreate.Description;
            product.ContainsAlcohol = productCreate.ContainsAlcohol;

            Console.WriteLine(product.Image);

            try
            {
                var result = await _productRepository.Create(product);
                return RedirectToAction("Get", new { id = product.Id });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Product kan niet aangemaakt worden, probeer het later nog eens.");
                return View(productCreate);
            }
        }

        public async Task<IActionResult> Get(string id)
        {
            if (!Guid.TryParse(id, out _))
            {
                return RedirectToAction("NotFound", "Home");
            }

            try
            {
                var productId = Guid.Parse(id);
                var product = await _productRepository.GetById(productId);

                if (product == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }

                return View(product);
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Home");
            }
        }
    }
}
