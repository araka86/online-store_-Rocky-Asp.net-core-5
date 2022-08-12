using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Model;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);
        IEnumerable<SelectListItem> GetAllDropdownList(string obj);
    }
}
