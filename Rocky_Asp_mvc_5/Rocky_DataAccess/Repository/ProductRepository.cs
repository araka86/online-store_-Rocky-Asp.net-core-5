using Rocky_Model;
using Rocky_DataAccess.Repository.IReposotory;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Utility;
namespace Rocky_DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetAllDropdownList(string obj)
        {
            //DropDownList
           if(obj == WebConstanta.CategoryName)
            {
                return _db.Categorys.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            if (obj == WebConstanta.AplicationTypeName)
            {
                return _db.AplicationType.Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
            }
            return null;
        }
        public void Update(Product obj)
        {       
            _db.Product.Update(obj); 
        }       
    }
}
