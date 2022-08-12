using Microsoft.AspNetCore.Mvc;
using Rocky_DataAccess.Repository.IReposotory;
using Rocky_Model.Viewmodels;
using Rocky_Utility;
using Rocky_Utility.BrainTree;
using System.Linq;
using System;
using Rocky_Model;
using Braintree;
using Microsoft.AspNetCore.Authorization;
namespace Rocky2.Controllers
{
    [Authorize(Roles = WebConstanta.AdminRole)]
    public class OrderController : Controller
    {
        private readonly IOrderHeaderRepository _orderHRepo;
        private readonly IOrderDetailRepository _orderDRepo;
        private readonly IBrainTreeGate _brain;
        [BindProperty]
        public OrderVM OrderVM { get; set; }
        public OrderController(IOrderHeaderRepository orderHRepo,
                                IOrderDetailRepository orderDRepo,
                                IBrainTreeGate brain
                                )
        {
            _orderHRepo = orderHRepo;
            _orderDRepo = orderDRepo;
            _brain = brain;
        }
        public IActionResult Index(string searchName=null, string searchEmail=null, string searchPhone=null, string Status=null)
        {
            var testEnum = Enum.GetValues(typeof(ListStatus1)).Cast<ListStatus1>();
            OrderListVm orderListVm = new OrderListVm()
            {
                OrderHeaderList = _orderHRepo.GetAll()
            };
            foreach (var currentType in Enum.GetValues(typeof(ListStatus1)))
            {
                var sortType = currentType.ToString();
            }
            if (!string.IsNullOrEmpty(searchName))
            {
                orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(u => u.FullName.ToLower().Contains(searchName.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchEmail))
            {
                orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower()));
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(u => u.PhoneNumber.ToLower().Contains(searchPhone.ToLower()));
            }
            if (!string.IsNullOrEmpty(Status) && Status!= "--Order Status--")
            {
                orderListVm.OrderHeaderList = orderListVm.OrderHeaderList.Where(u => u.OrderStatus.ToLower().Contains(Status.ToLower()));
            }
            return View(orderListVm);
        }
        public IActionResult Details(int id)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _orderHRepo.FirstOrDefault(u => u.Id == id),
                //для каждого елемента из OrderDetail явно включенна ссилка на Product
                OrderDetails = _orderDRepo.GetAll(o => o.OrderHeaderId == id, includeProperties: "Product") 
            };
            return View(orderVM);
        }
        [HttpPost]
        public IActionResult StartProcessing()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);// получаем обьект OrderHeader
            orderHeader.OrderStatus = WebConstanta.StatusProcessing;
            _orderHRepo.Save();
            TempData[WebConstanta.Success] = "Order StartProcessing Successfuly!!!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult ShipOrder() 
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeader.OrderStatus = WebConstanta.StatusShipped;
            orderHeader.ShippingDate = DateTime.Now;
            _orderHRepo.Save();
            TempData[WebConstanta.Success] = "Order Shipped Successfuly!!!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult CancelOrder()
        {
            OrderHeader orderHeader = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);          
            var gateway  = _brain.GetGateway();
            Transaction transaction = gateway.Transaction.Find(orderHeader.TransactionId);
            if(transaction.Status == TransactionStatus.AUTHORIZED || transaction.Status == TransactionStatus.SUBMITTED_FOR_SETTLEMENT)
            {
                //no refund
                Result<Transaction> resultvoid = gateway.Transaction.Void(orderHeader.TransactionId);
            }
            else
            {
                Result<Transaction> resultRefund = gateway.Transaction.Refund(orderHeader.TransactionId);
            }
            orderHeader.OrderStatus = WebConstanta.StatusRefunded;
            _orderHRepo.Save();
            TempData[WebConstanta.Success] = "Order Cancelled Successfuly!!!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult UpdateOrderDetails()
        {
            OrderHeader orderHeaderFromDb = _orderHRepo.FirstOrDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.FullName = OrderVM.OrderHeader.FullName;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;
            orderHeaderFromDb.Email = OrderVM.OrderHeader.Email;
            _orderHRepo.Save();
            TempData[WebConstanta.Success] = "Order Detail Updated Successfuly!!!";
            return RedirectToAction("Details","Order", new {id=orderHeaderFromDb.Id});
        }
    }
}
