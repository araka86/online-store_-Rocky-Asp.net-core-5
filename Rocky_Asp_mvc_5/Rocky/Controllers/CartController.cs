using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky_Model;
using Rocky_Model.Viewmodels;
using Rocky_Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Rocky_DataAccess.Repository.IReposotory;
using System;
using Rocky_Utility.BrainTree;
using Microsoft.AspNetCore.Http;
using Braintree;
namespace Rocky2.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnviroment;
        private readonly IEmailSender _emailSender;
        private readonly IAplicationUserRepository _useeRepo;
        private readonly IProductRepository _prodRepo;
        private readonly IInquiryHeaderRepository _inqHRepo;
        private readonly IInquiryDetailRepository _inqDRepo;
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;

        [BindProperty] //for post requests (will be available by default in Summary post)
        public ProductUserViewModel productuserViewModel { get; set; }
        public CartController(IWebHostEnvironment webHostEnvironment,
                                IEmailSender emailSender,
                                IAplicationUserRepository useeRepo,
                                IProductRepository prodRepo,
                                IInquiryHeaderRepository inqHRepo,
                                IInquiryDetailRepository inqDRepo,
                                IOrderHeaderRepository orderHRepo,
                                IOrderDetailRepository orderDRepo,
                                IBrainTreeGate brain
                                )
        {

            _webHostEnviroment = webHostEnvironment;
            _emailSender = emailSender;
            _useeRepo = useeRepo;
            _prodRepo = prodRepo;
            _inqHRepo = inqHRepo;
            _inqDRepo = inqDRepo;
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }

        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCarts = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                //session exist
                shoppingCarts = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).ToList();
            }
            List<int> productCart = shoppingCarts.Select(x => x.ProductId).ToList();

            IEnumerable<Product> productsListTemp = _prodRepo.GetAll(u => productCart.Contains(u.Id)); //temporary list
            IList<Product> productsList = new List<Product>(); //the final list for transfer to View
            foreach (var cartObj in shoppingCarts)
            {
                Product prodtemp = productsListTemp.FirstOrDefault(u => u.Id == cartObj.ProductId); //get the object prodtemp
                prodtemp.TempSqFt = cartObj.Sqft;
                productsList.Add(prodtemp);
            }
            return View(productsList);
        }

        //continue button
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            //to search for all Product objects from our list
            foreach (Product prod in prodList)
            {
                shoppingCartsList.Add(new ShoppingCart
                {
                    ProductId = prod.Id,
                    Sqft = prod.TempSqFt
                });
            }
            //set the value for the current session
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartsList);
            return RedirectToAction(nameof(Summary));
        }
        public IActionResult Summary()
        {
            AplicationUser aplicationUser;
            // Filling user data in the cart
            if (User.IsInRole(WebConstanta.AdminRole))
            {
                //Check the values for the session
                if (HttpContext.Session.Get<int>(WebConstanta.SessionInquiryId) != 0) //If it is not 0, it means that some query is being processed
                {
                    //cart has been loaded using an inquiry
                    InquiryHeader inquiryHeader = _inqHRepo.FirstOrDefault(u => u.Id == HttpContext.Session.Get<int>(WebConstanta.SessionInquiryId));
                    //filling the cart based on the current request
                    aplicationUser = new AplicationUser()
                    {
                        Email = inquiryHeader.Email,
                        FullName = inquiryHeader.FullName,
                        PhoneNumber = inquiryHeader.PhoneNumber
                    };
                }
                else
                {
                    //When an administrator places an order for a customer who has just come into the store without using the site
                    aplicationUser = new AplicationUser();
                }
                //Generating, assigning, and retrieving a client token to send it to javascript ---------------->(Brain Tree)<----------------
                var gateway = _brain.GetGateway();
                var clienttoken = gateway.ClientToken.Generate(); //Token Cient
                ViewBag.ClientToken = clienttoken;
            }
            else
            {
                // data user (name, mail,tel)
                //Get  id user (object will be received if the user is logged in)
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.Claims.FirstOrDefault();
                aplicationUser = _useeRepo.FirstOrDefault(u => u.Id == claim.Value);
            }
            //access to the shopping cart(access a session, load a list from sessions and retrieve a list of items in the shopping cart based on that session)
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                //session exist
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).ToList();
            }
            List<int> productCart = shoppingCartList.Select(x => x.ProductId).ToList();
            IEnumerable<Product> productsList = _prodRepo.GetAll(u => productCart.Contains(u.Id));
            productuserViewModel = new ProductUserViewModel()
            {
                AplicationUser = aplicationUser
            };
            //update data (get count)
            foreach (var cartObj in shoppingCartList)
            {
                Product prodTemp = _prodRepo.FirstOrDefault(u => u.Id == cartObj.ProductId);
                prodTemp.TempSqFt = cartObj.Sqft;
                productuserViewModel.ProductList.Add(prodTemp);
            }
            return View(productuserViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(IFormCollection collection, ProductUserViewModel productUserViewModel) //  а так же будет доступ через привязаное свойство  productuserViewModel
        {
            //BrainTree - when receiving data from the form, we also get the value of the nonce SummaryPost token
            //Get id  customer
            var claimsIndentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIndentity.FindFirst(ClaimTypes.NameIdentifier); //искомое значение
            if (User.IsInRole(WebConstanta.AdminRole))
            {
                //we need to create an order (for create order)
                OrderHeader orderHeader = new OrderHeader()
                {
                    CreatedByUserId = claim.Value, //содержит id текущего пользователя
                    FinalOrderTotal = productuserViewModel.ProductList.Sum(x => x.TempSqFt * x.Price), // total goods(Linq)
                    City = productuserViewModel.AplicationUser.City,
                    StreetAddress = productuserViewModel.AplicationUser.StreetAddress,
                    State = productuserViewModel.AplicationUser.State,
                    PostalCode = productuserViewModel.AplicationUser.PostalCode,
                    FullName = productuserViewModel.AplicationUser.FullName,
                    Email = productuserViewModel.AplicationUser.Email,
                    PhoneNumber = productuserViewModel.AplicationUser.PhoneNumber,
                    OrderDate = DateTime.Now,
                    OrderStatus = WebConstanta.StatusPending
                };
                //add orderHeader in database
                _orderHRepo.Add(orderHeader);
                _orderHRepo.Save();
                //Each product in the ProductList must be added to the OrderDetail
                foreach (var product in productUserViewModel.ProductList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        OrderHeaderId = orderHeader.Id,
                        PricePerSqFt = product.Price,
                        Sqft = product.TempSqFt,
                        ProductId = product.Id
                    };
                    _orderDRepo.Add(orderDetail);
                }
                _orderDRepo.Save();
                //getting the token value as a string from the collection ----->BrainTree<---------------
                string nonceFromTheClient = collection["payment_method_nonce"];
                //transaction creation
                var request = new TransactionRequest
                {
                    Amount = Convert.ToDecimal(orderHeader.FinalOrderTotal), //payment amount
                    PaymentMethodNonce = nonceFromTheClient,
                    OrderId = orderHeader.Id.ToString(),

                    Options = new TransactionOptionsRequest
                    {
                        SubmitForSettlement = true // for BrainTree system is set that when a transaction is requested, there is an automatic confirmation
                    }
                };
                var gateway = _brain.GetGateway(); //gaining access to the gateway
                Result<Transaction> result = gateway.Transaction.Sale(request); //method from BrainTree will be called to perform a transaction on our request (getting the result of the transaction)
                //order status changes
                if (result.Target.ProcessorResponseText == "Approved")
                {
                    orderHeader.TransactionId = result.Target.Id;
                    orderHeader.OrderStatus = WebConstanta.StatusApproved;
                }
                else
                {
                    orderHeader.OrderStatus = WebConstanta.StatusCancelled;
                }
                _orderHRepo.Save();
                return RedirectToAction(nameof(InquiryConfirmation), new { id = orderHeader.Id });
            }
            else
            {
                //we need to create an inquiry (для создания запроса)
                //template path
                var PathToTemplate = _webHostEnviroment.WebRootPath
                    + Path.DirectorySeparatorChar.ToString()
                    + "templates" + Path.DirectorySeparatorChar.ToString()
                    + "Inquiry.html";

                var subject = "New Inquiry";
                string HtmlBody = "";
                using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
                {
                    HtmlBody = sr.ReadToEnd();
                }
                //Name:     {0}
                //Email:    {1}
                //Phone:    {2}
                //Products: {3}
                StringBuilder productListSB = new StringBuilder();

                foreach (var product in productUserViewModel.ProductList)
                    productListSB.Append($"- Name {product.Name}<span style='font-size:14px;' >(ID: {product.Id})</span><br />");

                string messageBody = string.Format(HtmlBody,
                    productUserViewModel.AplicationUser.FullName,
                    productUserViewModel.AplicationUser.Email,
                    productUserViewModel.AplicationUser.PhoneNumber,
                    productListSB.ToString());
                await _emailSender.SendEmailAsync(WebConstanta.EmailAdmin, subject, messageBody);
                //After successfully sending an email, you need to pass the query header and query details to the database.
                InquiryHeader inquiryHeader = new InquiryHeader()
                {
                    AplicationUserId = claim.Value,
                    FullName = productuserViewModel.AplicationUser.FullName,
                    Email = productuserViewModel.AplicationUser.Email,
                    PhoneNumber = productuserViewModel.AplicationUser.PhoneNumber,
                    InquiryDate = DateTime.Now
                };
                _inqHRepo.Add(inquiryHeader);
                _inqHRepo.Save();
                //when there is more than 1 item. For each of these items you need to fill InquiryDetail
                foreach (var product in productUserViewModel.ProductList)
                {
                    InquiryDetail inquiryDetail = new InquiryDetail()
                    {
                        InquiryHeaderId = inquiryHeader.Id,
                        ProductId = product.Id
                    };
                    _inqDRepo.Add(inquiryDetail);
                }
                _inqDRepo.Save();
                TempData[WebConstanta.Success] = "Inquiry submited successfully";
                // When a message is sent, you need to load a confirmation page to show the client the request being sent
            }
            return RedirectToAction(nameof(InquiryConfirmation));
        }
        //View Confirm
        //If the id has a valid value, it means that the method is called for a situation with an order placed (in the case of an admin)
        public IActionResult InquiryConfirmation(int id = 0)
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id);
            //clearing the data of the current session. Since for the current session, all the products that the client was interested in are already included in the query
            HttpContext.Session.Clear();
            return View(orderHeader);
        }
        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            if (HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart) != null &&
               HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).Count() > 0)
            {
                //session exist
                shoppingCartsList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WebConstanta.SessionCart).ToList();
            }
            shoppingCartsList.Remove(shoppingCartsList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartsList); //Setting up a session after deletion
            TempData[WebConstanta.Success] = "Product Remote to cart successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult RemoveAll(int id)
        {
            HttpContext.Session.Clear();
            TempData[WebConstanta.Success] = "All Product Remote to cart successfully";
            return RedirectToAction("Index", "Home");
        }
        //update Cart
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult UpdateCart(IEnumerable<Product> prodList)
        {
            List<ShoppingCart> shoppingCartsList = new List<ShoppingCart>();
            //to search for all Product objects from our list
            foreach (Product prod in prodList)
            {
                shoppingCartsList.Add(new ShoppingCart
                {
                    ProductId = prod.Id,
                    Sqft = prod.TempSqFt
                });
            }
            //set the value for the current session
            HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartsList);
            return RedirectToAction(nameof(Index));
        }
    }
}