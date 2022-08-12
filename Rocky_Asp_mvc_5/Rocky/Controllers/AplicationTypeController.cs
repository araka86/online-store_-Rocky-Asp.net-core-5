using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_Utility;
using Rocky_Model;
using System.Collections.Generic;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky2.Controllers;
[Authorize(Roles = WebConstanta.AdminRole)]
public class AplicationTypeController : Controller
{
    private readonly IAplicationTypeRepository _apRepo;       
    public AplicationTypeController(IAplicationTypeRepository apRepo) => _apRepo = apRepo;
    public IActionResult Index()
    {
        IEnumerable<AplicationType> objtList = _apRepo.GetAll();
        return View(objtList);
    }
    //Get - Greate
    public IActionResult Create() => View();  
    //Post - Greate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(AplicationType obj)
    {
        if (ModelState.IsValid)
        {
            _apRepo.Add(obj);
            _apRepo.Save();
            TempData[WebConstanta.Success] = "AplicationType create successfully";
            return Redirect("Index");
        }
        TempData[WebConstanta.Error] = "AplicationType create Error";
        return View(obj);
    }
    //Get - Edit
    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var obj = _apRepo.Find(id.GetValueOrDefault());

        if (obj == null)
            return NotFound();

        return View(obj);
    }
    //Post - Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(AplicationType obj)
    {
        if (ModelState.IsValid)
        {
            _apRepo.Update(obj);
            _apRepo.Save();
            TempData[WebConstanta.Success] = "AplicationType Update successfully";
            return RedirectToAction("Index");
        }
        TempData[WebConstanta.Error] = "AplicationType Update Error";
        return View(obj);
    }
    //Get - Delete
    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
            return NotFound();

        var obj = _apRepo.Find(id.GetValueOrDefault());

        if (obj == null)
            return NotFound();

        return View(obj);
    }
    //Post - Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeletePost(int? id)
    {
        var obj = _apRepo.Find(id.GetValueOrDefault());
        if (obj == null)
            return NotFound();

        _apRepo.Remove(obj);
        _apRepo.Save();
        TempData[WebConstanta.Success] = "AplicationType Delete successfully";
        return RedirectToAction("Index");
    }
}
