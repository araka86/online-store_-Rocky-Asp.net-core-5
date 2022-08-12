using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rocky_Model;
using Rocky_Model.Viewmodels;
using Rocky_Utility;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using Rocky_DataAccess;
using Rocky_DataAccess.Repository;
using Rocky_DataAccess.Repository.IReposotory;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Rocky2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _prodRepo;
        private readonly ICategoryRepository _catRepo;
        private readonly IAplicationUserRepository _useeRepo;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserRepo _userRepo;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IQueryable<AplicationUser> query; 
        public HomeController(ILogger<HomeController> logger,
            IProductRepository prodRepo,
            ICategoryRepository catRepo,
            IAplicationUserRepository useeRepo,
            SignInManager<IdentityUser> signInManager,
            UserRepo userRepo,
            ApplicationDbContext applicationDbContext)
        {
          //  Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            _logger = logger;
            _prodRepo = prodRepo;
            _catRepo = catRepo;
            _useeRepo = useeRepo;
            _signInManager = signInManager;
            _userRepo = userRepo;
            _applicationDbContext = applicationDbContext;
            query = _applicationDbContext.Set<AplicationUser>();
        }
        public IActionResult Index()
        {
            HomeVm homeVm = new HomeVm()
            {
                Products = _prodRepo.GetAll(includeProperties: "Category,AplicationType"),
                Categories = _catRepo.GetAll()

            };
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier); 
            if (_signInManager.IsSignedIn(User))
            {
                IQueryable<AplicationUser> FindUserFullName = (IQueryable<AplicationUser>)_userRepo.FirstOrDefault( claim.Value);

                foreach (var tt in FindUserFullName) 
                     ViewBag.Name = tt.FullName.ToString();
  
            }
            return View(homeVm);
        }
        public IActionResult Details(int id)
        {
            //take session
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstanta.SessionCart);
            }

            DetailsVM DetailsVM = new DetailsVM()
            {
                Product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties: "Category,AplicationType"),
                ExistInCart = false
            };

            //check item session
            foreach (var item in shoppingCartList)
            {
                if (item.ProductId == id)
                    DetailsVM.ExistInCart = true;
            }

            return View(DetailsVM);
        }
        //Add Datail to cart
        [HttpPost, ActionName("Details")]
        public IActionResult DetailsPost(int id, DetailsVM detailsVM)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstanta.SessionCart);
            }
            shoppingCartList.Add(new ShoppingCart { ProductId = id, Sqft = detailsVM.Product.TempSqFt });
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartList);
            TempData[WebConstanta.Success] = "Product Add to cart successfully";
            return RedirectToAction(nameof(Index));
        }
        //Delete Detail
        public IActionResult RemoveFromCart(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                shoppingCartList = HttpContext.Session.Get<List<ShoppingCart>>(WebConstanta.SessionCart);
            }
            var itemToRemove = shoppingCartList.SingleOrDefault(r => r.ProductId == id);
            if (itemToRemove != null)
                shoppingCartList.Remove(itemToRemove);


            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartList);
            TempData[WebConstanta.Success] = "Product remote to cart successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
