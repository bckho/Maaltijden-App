using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using MaaltijdenApp_WebApp.Infrastructure;
using MaaltijdenApp_WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaaltijdenApp_WebApp.Controllers
{
    /// <summary>
    /// MealPackageController manages the MealPackage entity.
    /// </summary>
    [Authorize]
    public class MealPackageController : Controller
    {
        private readonly UserManager<IdentityUser> _userMgr;
        private readonly ICanteenRepository _canteenRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMealPackageRepository _mealPackageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStudentRepository _studentRepository;

        public MealPackageController(
            UserManager<IdentityUser> userMgr,
            ICanteenRepository canteenRepository,
            IEmployeeRepository employeeRepository,
            IMealPackageRepository mealPackageRepository,
            IProductRepository productRepository,
            IStudentRepository studentRepository)
        {
            _userMgr = userMgr;
            _canteenRepository = canteenRepository;
            _employeeRepository = employeeRepository;
            _mealPackageRepository = mealPackageRepository;
            _productRepository = productRepository;
            _studentRepository = studentRepository;
        }

        /// <summary>
        /// Index returns view with list of MealPackages
        /// </summary>
        /// <returns>View</returns>
        public async Task<IActionResult> Index()
        {
            var mp = _mealPackageRepository.GetAllFuturePlanned();

            var im = new IndexModel();

            im.MealPackages = mp;

            // If User is student, then only return list of available MealPackages.
            if (User.IsInRole("student"))
            {
                var f = mp.Where(m => m.ReservedByStudentId == null);

                im.IsReserved = false;
                im.MealPackages = f;
            }
            else if (User.IsInRole("employee"))
            {
                // Get current authenticated employee user
                var currentUser = await _userMgr.GetUserAsync(User);
                var employee = await _employeeRepository.GetByEmail(currentUser.Email);

                if (employee != null && employee.Canteen != null)
                {
                    ViewBag.City = employee.Canteen.City;

                    var f = mp.Where(m => m.Canteen!.City == employee.Canteen.City);

                    im.CanteenCity = employee.Canteen.City;
                    im.MealPackages = f;
                }
            }

            return View(im);
        }

        /// <summary>
        /// Index returns view with list of MealPackages when filters are applied.
        /// </summary>
        /// <returns>View</returns>
        [HttpPost]
        public IActionResult Index(IndexModel filter)
        {
            var mp = _mealPackageRepository.GetAllFuturePlanned();

            if (filter.CanteenCity != null && filter.IsReserved == null)
            {
                var filtered = mp.Where(m => m.Canteen!.City == filter.CanteenCity);
                if (User.IsInRole("student"))
                {
                    filtered = filtered.Where(m => m.ReservedByStudentId != null);
                }
                
                filter.MealPackages = filtered;
                return View(filter);
            }

            if (filter.CanteenCity != null && filter.IsReserved == false)
            {
                var filtered = mp.Where(m => m.Canteen!.City == filter.CanteenCity && m.ReservedByStudentId == null);
                filter.MealPackages = filtered;
                return View(filter);
            }

            if (filter.CanteenCity != null && filter.IsReserved == true)
            {
                var filtered = mp.Where(m => m.Canteen!.City == filter.CanteenCity && m.ReservedByStudentId != null);
                filter.MealPackages = filtered;
                return View(filter);
            }

            if (filter.CanteenCity == null && filter.IsReserved != null)
            {
                if (filter.IsReserved == true)
                {
                    var filtered = mp.Where(m => m.ReservedByStudentId != null);
                    filter.MealPackages = filtered;
                    return View(filter);
                }
                else
                {
                    var filtered = mp.Where(m => m.ReservedByStudentId == null);
                    filter.MealPackages = filtered;
                    return View(filter);
                }
            }

            filter.MealPackages = mp;

            return View(filter);
        }

        /// <summary>
        /// Returns view for creating a new MealPackage with selectable data.
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles = "employee")]
        public async Task<ActionResult> Initialize()
        {
            // Clear out session data
            HttpContext.Session.Remove("InitializeMealPackage");
            HttpContext.Session.Remove("Cart");

            // Get current authenticated employee user
            var currentUser = await _userMgr.GetUserAsync(User);
            var employee = await _employeeRepository.GetByEmail(currentUser.Email);

            // Check if employee exists
            if (employee == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var canteen = employee.Canteen;

            // Check if canteen of current employee can serve hot meals
            if (canteen != null && canteen.CanServeHotDinnerMeals)
            {
                ViewBag.CanServeHotMeal = true;
            }
            else
            {
                ViewBag.CanServeHotMeal = false;
            }

            ViewBag.Canteen = employee.Canteen;

            return View();
        }

        /// <summary>
        /// Action after submitting form data from the Initialize view.
        /// Stores data temporarily in session data.
        /// </summary>
        /// <param name="mealPackageCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> Initialize(InitializeMealPackage mealPackageCreate)
        {
            // Get current authenticated user
            var currentUser = await _userMgr.GetUserAsync(User);
            var employee = await _employeeRepository.GetByEmail(currentUser.Email);

            if (employee == null || employee.Canteen == null)
            {
                return RedirectToAction("NotFound", "Home");
            }

            if (ModelState.IsValid)
            {
                // Check if pickup times are valid.
                if (!mealPackageCreate.PickUpDateIsValid())
                {
                    ModelState.AddModelError("", "Ophaaltijd gegevens zijn ongeldig. Voer geldige gegevens in.");
                    return View(mealPackageCreate);
                }

                // Check if user exists and works in a canteen.
                if (employee == null || employee.Canteen == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }

                // Clear out session json
                HttpContext.Session.Remove("InitializeUpdateMealPackage");

                // Store MealPackageCreate in Session.
                HttpContext.Session.SetJson("InitializeMealPackage", mealPackageCreate);

                return RedirectToAction("SelectProduct", "MealPackage");
            }

            var canteen = employee.Canteen;

            // Check if canteen of current employee can serve hot meals
            if (canteen != null && canteen.CanServeHotDinnerMeals)
            {
                ViewBag.CanServeHotMeal = true;
            }
            else
            {
                ViewBag.CanServeHotMeal = false;
            }

            ViewBag.Canteen = employee.Canteen;

            ModelState.AddModelError("", "Voer geldige gegevens in.");

            return View(mealPackageCreate);
        }

        /// <summary>
        /// SelectProduct returns a view where products are shown that can be added to the cart.
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles = "employee")]
        public IActionResult SelectProduct()
        {
            var products = _productRepository.GetAll().ToList();

            // Only display products that are not added to the cart yet.
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            if (cart.Any())
            {
                // Filter out already added products.
                var filtered = products.Where(x => !cart.Any(c => c.ProductId == x.Id));

                return View(filtered.AsQueryable());
            }
            else
            {
                // Return all products if cart is empty.
                return View(products.AsQueryable());
            }

        }

        /// <summary>
        /// Create() attemps to register new MealPackage with given MealPackageCreate model and stored CartItems in Session.
        /// </summary>
        /// <param name="mealPackageCreate"></param>
        /// <returns>RedirectToAction View</returns>
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> Create()
        {
            if (ModelState.IsValid)
            {
                // Get current employee information.
                var currentUser = await _userMgr.GetUserAsync(User);
                var employee = await _employeeRepository.GetByEmail(currentUser.Email);

                if (employee == null || employee.Canteen == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }

                // Get InitializeMealPackage from Session.
                var initMP = HttpContext.Session.GetJson<InitializeMealPackage>("InitializeMealPackage");

                // Check if InitializeMealPackage is not null.
                if (initMP == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }

                // Get Cart from Session.
                List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // Check if Cart is not empty.
                if (!cart.Any())
                {
                    return RedirectToAction("NotFound", "Home");
                }

                // Create list for products
                var products = new List<Product>();

                // Get product row for each cart item
                foreach (var cartItem in cart)
                {
                    var p = await _productRepository.GetById(cartItem.ProductId);
                    if (p != null)
                    {
                        products.Add(p);
                    }
                }

                // Create new MealPackage
                var mealPackage = new MealPackage
                {
                    Id = Guid.NewGuid(),
                    City = employee.Canteen.City,
                    CanteenId = employee.CanteenId,
                    Canteen = employee.Canteen,
                    StartPickupDateTime = initMP.StartPickupDateTime,
                    EndPickupDateTime = initMP.EndPickupDateTime,
                    Price = initMP.Price,
                    MealType = initMP.MealType,
                    IsHotMeal = initMP.IsHotMeal,
                    IsEighteenPlusOnly = initMP.IsEighteenPlusOnly,
                    ProductsIndicator = products
                };

                try
                {
                    // Insert new MealPackage row in db.
                    var result = await _mealPackageRepository.Create(mealPackage);

                    // Clear out session json
                    HttpContext.Session.Remove("InitializeMealPackage");
                    HttpContext.Session.Remove("InitializeUpdateMealPackage");

                    return RedirectToAction("Get", "MealPackage", new { Id = mealPackage.Id.ToString() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            }

            return RedirectToAction("SelectProduct", "MealPackage");
        }

        /// <summary>
        /// Get MealPackage detailed view by id.
        /// </summary>
        /// <param name="id">MealPackage Id</param>
        /// <returns>View</returns>
        public async Task<ActionResult> Get(string id)
        {
            // Clear out session json
            HttpContext.Session.Remove("InitializeMealPackage");
            HttpContext.Session.Remove("InitializeUpdateMealPackage");

            // Check if id is valid GUID
            if (!Guid.TryParse(id, out _))
            {
                return RedirectToAction("NotFound", "Home");
            }

            try
            {
                var mealPackageId = Guid.Parse(id);

                // Get MealPackage by id.
                var mealPackage = await _mealPackageRepository.GetById(mealPackageId);

                if (mealPackage != null)
                {
                    return View(mealPackage);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Home");
            }

            return RedirectToAction("NotFound", "Home");
        }

        /// <summary>
        /// Add student reservation to designated MealPackage.
        /// </summary>
        /// <param name="mealPackageId">MealPackage ID</param>
        /// <returns>View</returns>
        [Authorize(Roles = "student")]
        public async Task<ActionResult> CreateReservation(string id)
        {
            // Check if id is valid GUID
            if (!Guid.TryParse(id, out _)) return RedirectToAction("NotFound", "Home");

            var mealPackageId = Guid.Parse(id);

            try
            {
                // Get student account from current authenticated user.
                var currentUser = await _userMgr.GetUserAsync(User);
                var student = await _studentRepository.GetByEmail(currentUser.Email);

                // Check if student exists.
                if (student == null) return RedirectToAction("NotFound", "Home");

                // Get MealPackage
                var mealPackage = await _mealPackageRepository.GetById(mealPackageId);

                // Check if MealPackage exists.
                if (mealPackage == null) return RedirectToAction("NotFound", "Home");

                // Check if MealPackage is already reserved by another student.
                if (mealPackage.ReservedByStudentId != null)
                {
                    TempData["var"] = "Dit maaltijdpakket is helaas al gereserveerd.";
                }

                // Create reservation
                var result = await _mealPackageRepository.CreateStudentReservation(mealPackageId, student.Id);

                TempData["reservation"] = "Maaltijdpakket gereserveerd!";
            }
            catch (Exception e)
            {
                // Error
                TempData["var"] = e.Message;
            }

            return RedirectToAction("Get", "MealPackage", new { Id = mealPackageId.ToString() });
        }

        /// <summary>
        /// InitializeUpdate returns view where employee can update existing MealPackage.
        /// </summary>
        /// <param name="id">MealPackageId (GUID)</param>
        /// <returns>View</returns>
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> InitializeUpdate(string id)
        {
            // Clear out session json
            HttpContext.Session.Remove("InitializeMealPackage");
            HttpContext.Session.Remove("InitializeUpdateMealPackage");

            // Check if id is valid GUID
            if (!Guid.TryParse(id, out _)) return RedirectToAction("NotFound", "Home");

            var mealPackageId = Guid.Parse(id);

            // Get MealPackage by id
            var mp = await _mealPackageRepository.GetById(mealPackageId);

            if (mp == null || mp.Canteen == null) return RedirectToAction("NotFound", "Home");

            // If MealPackage is already reserved, return to MealPackage page and show error.
            if (mp.ReservedByStudentId != null)
            {
                // Error message
                TempData["ErrorMessage"] = "Maaltijdpakket is al gereserveerd. Wijzigen aanbrengen is niet meer mogelijk.";

                return RedirectToAction("Get", "MealPackage", new { Id = mealPackageId.ToString() });
            }

            // Translate MealPackage to ViewModel
            var initMP = new InitializeUpdateMealPackage
            {
                Id = mp.Id,
                Canteen = mp.Canteen,
                StartPickupDateTime = mp.StartPickupDateTime,
                EndPickupDateTime = mp.EndPickupDateTime,
                Price = mp.Price,
                MealType = mp.MealType,
                IsHotMeal = mp.IsHotMeal,
                IsEighteenPlusOnly = mp.IsEighteenPlusOnly
            };

            var canteen = mp.Canteen;

            // Check if canteen of current employee can serve hot meals
            if (canteen != null && canteen.CanServeHotDinnerMeals)
            {
                ViewBag.CanServeHotMeal = true;
            }
            else
            {
                ViewBag.CanServeHotMeal = false;
            }

            ViewBag.Canteen = canteen;

            return View(initMP);
        }

        /// <summary>
        /// Action after submitting form data from the InitializeUpdate view.
        /// Stores data temporarily in session data.
        /// </summary>
        /// <param name="mealPackageUpdate"></param>
        /// <returns>Redirected View</returns>
        [HttpPost]
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> InitializeUpdate(InitializeUpdateMealPackage mealPackageUpdate)
        {
            // Get products from current MealPackage
            var mp = await _mealPackageRepository.GetById(mealPackageUpdate.Id);

            if (mp == null) return RedirectToAction("NotFound", "Home");

            mealPackageUpdate.Canteen = mp.Canteen;

            if (ModelState.IsValid)
            {
                // Check if pickup times are valid.
                if (!mealPackageUpdate.PickUpDateIsValid())
                {
                    ModelState.AddModelError("", "Ophaaltijd gegevens zijn ongeldig. Voer geldige gegevens in.");
                    return View(mealPackageUpdate);
                }

                // Store MealPackageCreate in Session.
                HttpContext.Session.SetJson("InitializeUpdateMealPackage", mealPackageUpdate);

                // Only display products that are not added to the cart yet.
                List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                foreach (var p in mp.ProductsIndicator)
                {
                    var ci = new CartItem
                    {
                        ProductId = p.Id,
                        ProductName = p.Name
                    };
                    cart.Add(ci);
                }

                // Clear out session json
                HttpContext.Session.Remove("InitializeMealPackage");

                // Save product items to cart json
                HttpContext.Session.SetJson("Cart", cart);


                return RedirectToAction("SelectProduct", "MealPackage");
            }

            var canteen = mp.Canteen;

            // Check if canteen of current employee can serve hot meals
            if (canteen != null && canteen.CanServeHotDinnerMeals)
            {
                ViewBag.CanServeHotMeal = true;
            }
            else
            {
                ViewBag.CanServeHotMeal = false;
            }

            ViewBag.Canteen = canteen;

            ModelState.AddModelError("", "Voer geldige gegevens in.");

            return View(mealPackageUpdate);
        }

        /// <summary>
        /// Updates a MealPackage.
        /// </summary>
        /// <returns>Redirected View</returns>
        [Authorize(Roles = "employee")]
        public async Task<IActionResult> Update()
        {
            if (ModelState.IsValid)
            {
                // Get InitializeUpdateMealPackage from Session.
                var initMP = HttpContext.Session.GetJson<InitializeUpdateMealPackage>("InitializeUpdateMealPackage");

                // Check if InitializeUpdateMealPackage is not null.
                if (initMP == null)
                {
                    return RedirectToAction("NotFound", "Home");
                }

                // Get MealPackage from DB
                var mp = await _mealPackageRepository.GetById(initMP.Id);

                // Check if MealPackage exists
                if (mp == null) return RedirectToAction("NotFound", "Home");

                // Get Cart from Session.
                List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // Check if Cart is not empty.
                if (!cart.Any())
                {
                    return RedirectToAction("SelectProduct", "MealPackage");
                }

                // Transform cartItems to Products
                var products = new List<Product>();
                foreach (var cartItem in cart)
                {
                    var p = await _productRepository.GetById(cartItem.ProductId);
                    if (p != null)
                    {
                        products.Add(p);
                    }
                }

                mp.ProductsIndicator = products;
                mp.Price = initMP.Price;
                mp.StartPickupDateTime = initMP.StartPickupDateTime;
                mp.EndPickupDateTime = initMP.EndPickupDateTime;
                mp.IsHotMeal = initMP.IsHotMeal;
                mp.IsHotMeal = initMP.IsHotMeal;

                try
                {
                    // Update MealPackage row in db.
                    var result = await _mealPackageRepository.Update(mp);

                    // Clear out session json
                    HttpContext.Session.Remove("InitializeMealPackage");
                    HttpContext.Session.Remove("InitializeUpdateMealPackage");

                    return RedirectToAction("Get", "MealPackage", new { Id = mp.Id.ToString() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }

            }

            return View("SelectProduct", "MealPackage");
        }

        [Authorize(Roles = "employee")]
        public async Task<IActionResult> Delete(string id)
        {
            // Check if id is valid GUID
            if (!Guid.TryParse(id, out _)) return RedirectToAction("NotFound", "Home");

            var mealPackageId = Guid.Parse(id);

            // Get MealPackage by Id
            var mp = await _mealPackageRepository.GetById(mealPackageId);

            // Check if MealPackage exists
            if (mp == null) return RedirectToAction("NotFound", "Home");

            // If MealPackage is already reserved, then alert user with message and return to MealPackage page.
            if (mp.ReservedByStudentId != null)
            {
                TempData["var"] = "Maaltijdpakket kan niet verwijderd worden, reservering is al aangemaakt.";

                return RedirectToAction("Get", "MealPackage", new { Id = mp.Id.ToString() });
            }

            try
            {
                var r = await _mealPackageRepository.Delete(mealPackageId);

                TempData["var"] = "Maaltijdpakket verwijderd.";

                return RedirectToAction("Index", "MealPackage");
            }
            catch (Exception)
            {
                return RedirectToAction("NotFound", "Home");
            }
        }

        /// <summary>
        /// Show made reservations of authenticated student user.
        /// </summary>
        /// <returns>View</returns>
        [Authorize(Roles = "student")]
        public async Task<IActionResult> Reservations()
        {
            // Get student account from current authenticated user.
            var currentUser = await _userMgr.GetUserAsync(User);
            var student = await _studentRepository.GetByEmail(currentUser.Email);

            if (student == null) return RedirectToAction("NotFound", "Home");

            var mps = _mealPackageRepository.GetReservedByStudentId(student.Id);

            return View(mps);
        }
    }
}
