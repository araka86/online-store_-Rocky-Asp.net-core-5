using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky_Utility;
using Rocky_Model;
using System.Collections.Generic;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky2.Controllers
{
    [Authorize(Roles = WebConstanta.AdminRole)]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _catRepo;

        public CategoryController(ICategoryRepository catRepo)
        {
            _catRepo = catRepo;
        }  
        public IActionResult Index()
        {
            IEnumerable<Category> objtList = _catRepo.GetAll();
            return View(objtList);
        }

        //Get - Greate
        public IActionResult Create()
        {  
            return View();
        }


        //Post - Greate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {

            if (ModelState.IsValid) 
            {
                _catRepo.Add(obj);
                _catRepo.Save();
                TempData[WebConstanta.Success] = "Catogory created successfully";
                return Redirect("Index");
            }
            TempData[WebConstanta.Error] = "Error while creating category ";
            return View(obj);
        }

        //Get - Edit
        public IActionResult Edit(int? id)
        {
            if(id == null || id == 0)
                return NotFound();
            

            var obj = _catRepo.Find(id.GetValueOrDefault());


            if (obj == null )
                return NotFound();


            
            return View(obj);
        }
        //Post - Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _catRepo.Update(obj);
                _catRepo.Save();
                TempData[WebConstanta.Success] = "Catogory update successfully";
                return RedirectToAction("Index");
            }
            TempData[WebConstanta.Error] = "Catogory update Error";
            return View(obj);
        }


        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var obj = _catRepo.Find(id.GetValueOrDefault());

            if (obj == null)
                return NotFound();


            return View(obj);
        }
        //Post - Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _catRepo.Find(id.GetValueOrDefault());
            if (obj == null)
                return NotFound();

            
                _catRepo.Remove(obj);
                _catRepo.Save();
            TempData[WebConstanta.Success] = "Catogory Delete successfully";
            return RedirectToAction("Index");
        }
    }
}
