using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_Utility;
using Rocky_Model;
using System.Collections.Generic;
using Rocky_DataAccess.Repository.IReposotory;
using Rocky_Model.Viewmodels;
namespace Rocky2.Controllers;
[Authorize(Roles = WebConstanta.AdminRole)]
public class InquiryController : Controller
{
    private readonly IInquiryHeaderRepository _InquiryHeaderRepository;
    private readonly IInquiryDetailRepository _InquiryDetailRepository;

    [BindProperty]
    public InquiryVM InquiryVM { get; set; }

    public InquiryController(IInquiryHeaderRepository inquiryHeaderRepository, IInquiryDetailRepository inquiryDetailRepository)
    {
        _InquiryHeaderRepository = inquiryHeaderRepository;
        _InquiryDetailRepository = inquiryDetailRepository;
    } 
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Details(int id)
    {
        this.InquiryVM = new InquiryVM()
        {
            InquiryHeader = _InquiryHeaderRepository.FirstOrDefault(x => x.Id== id),
            InquiryDetails = _InquiryDetailRepository.GetAll(x => x.InquiryHeaderId == id, includeProperties:"Product")
        };
        return View(InquiryVM);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Details()
    {
        List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
        InquiryVM.InquiryDetails = _InquiryDetailRepository.GetAll(u=>u.InquiryHeaderId == InquiryVM.InquiryHeader.Id);
        foreach (var detail in InquiryVM.InquiryDetails)
        {
            ShoppingCart shoppingCart = new ShoppingCart()
            {
                ProductId = detail.ProductId,
                Sqft = 1 //crutch
            };        
            shoppingCartList.Add(shoppingCart);
        }
        HttpContext.Session.Clear();
        HttpContext.Session.Set(WebConstanta.SessionCart, shoppingCartList); // для юзеров
        HttpContext.Session.Set(WebConstanta.SessionInquiryId, InquiryVM.InquiryHeader.Id);// для админов
        TempData[WebConstanta.Success] = "Prodocts Add to cart successfully";
        return RedirectToAction("Index", "Cart");
    }
    [HttpPost]
    public IActionResult Delete()
    {
        InquiryHeader inquiryHeader = _InquiryHeaderRepository.FirstOrDefault(u => u.Id == InquiryVM.InquiryHeader.Id);
        IEnumerable<InquiryDetail> inquiryDetails = _InquiryDetailRepository.GetAll(u=>u.InquiryHeaderId==InquiryVM.InquiryHeader.Id);
        _InquiryDetailRepository.RemoveRange(inquiryDetails);
        _InquiryHeaderRepository.Remove(inquiryHeader);
        _InquiryHeaderRepository.Save();
        TempData[WebConstanta.Success] = "Inquiry delete successfully";
        return RedirectToAction(nameof(Index));
    }
    #region API CALLS
    [HttpGet]
    public IActionResult GetInquiryList() 
    {
        return Json(new {data = _InquiryHeaderRepository.GetAll() });
    }
    #endregion
}

