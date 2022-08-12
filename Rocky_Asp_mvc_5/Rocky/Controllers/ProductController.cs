using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Rocky_Utility;
using Rocky_Model;
using Rocky_Model.Viewmodels;
using Rocky_DataAccess.Repository.IReposotory;

namespace Rocky2.Controllers
{
    [Authorize(Roles = WebConstanta.AdminRole)]
    public class ProductController : Controller
    {
        private readonly IProductRepository _prodRepo;

        //dependency injection
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository prodRepo, IWebHostEnvironment webHostEnvironment)
        {
            _prodRepo = prodRepo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objtList = _prodRepo.GetAll(includeProperties:"Category,AplicationType");
            return View(objtList);
        }
        //Get - Upsert(Views-->Index (create/update))
        public IActionResult Upsert(int? id)
        {
            //----------------->ViewModel<-----(52)--------
            ProdoctVM prodoctVM = new ProdoctVM()
            {
                Product = new Product(),
                CategorySelectList = _prodRepo.GetAllDropdownList(WebConstanta.CategoryName),
                AplicationTypeSelectList = _prodRepo.GetAllDropdownList(WebConstanta.AplicationTypeName)
            };
            if (id == null) //Check object
            {
                //this is for create
                return View(prodoctVM);
            }
            else
            {
                //update
                prodoctVM.Product = _prodRepo.Find(id.GetValueOrDefault());
                if (prodoctVM == null)
                {
                    return NotFound();
                }
                return View(prodoctVM);
            }
        }
        //Post - Upsert (Views-->Upsert(only UPDATE) )
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProdoctVM prodoctVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (prodoctVM.Product.Id == 0)
                {
                    //creating
                    string upload = webRootPath + WebConstanta.ImagePath; 
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    prodoctVM.Product.Image = fileName + extension;
                    _prodRepo.Add(prodoctVM.Product);
                    TempData[WebConstanta.Success] = "Prodoct Create successfully";
                }
                else
                {
                    //updating
                    var objFromDB = _prodRepo.FirstOrDefault(u => u.Id == prodoctVM.Product.Id, isTracking:false);
                    if (files.Count > 0)
                    {
                        //creating
                        string upload = webRootPath + WebConstanta.ImagePath; 
                        string fileName = Guid.NewGuid().ToString();
                        var oldFile = Path.Combine(upload, objFromDB.Image);
                        //check and dell old fille
                        if (System.IO.File.Exists(oldFile))
                            System.IO.File.Delete(oldFile);


                        string extension = Path.GetExtension(files[0].FileName);
                        using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(filestream);
                        }
                        prodoctVM.Product.Image = fileName + extension;
                    }
                    else
                    {
                        prodoctVM.Product.Image = objFromDB.Image;
                    }
                    TempData[WebConstanta.Success] = "Prodoct Update successfully";
                    _prodRepo.Update(prodoctVM.Product);
                }
                _prodRepo.Save();
                return RedirectToAction("Index"); //return to Action
            }
            //if no valid
            prodoctVM.CategorySelectList = _prodRepo.GetAllDropdownList(WebConstanta.CategoryName);
            prodoctVM.AplicationTypeSelectList = _prodRepo.GetAllDropdownList(WebConstanta.AplicationTypeName);
            return View(prodoctVM);
        }
        //Get - Delete
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            Product product = _prodRepo.FirstOrDefault(u => u.Id == id, includeProperties:"Category,AplicationType");
              

            if (product == null)
                return NotFound();


            return View(product);
        }
        //Post - Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {
            var obj = _prodRepo.Find(id.GetValueOrDefault());
            if (obj == null)
                return NotFound();



            string upload = _webHostEnvironment.WebRootPath + WebConstanta.ImagePath; //get path from image
            var oldFile = Path.Combine(upload, obj.Image); // link on the old fille
            //check and dell old fille
            if (System.IO.File.Exists(oldFile))
                System.IO.File.Delete(oldFile);


            _prodRepo.Remove(obj);
            _prodRepo.Save();
            TempData[WebConstanta.Success] = "Prodoct Delete successfully";
            return RedirectToAction("Index");
        }
    }
}
