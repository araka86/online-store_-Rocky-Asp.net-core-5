using Microsoft.AspNetCore.Mvc.Rendering;
namespace Rocky_Model.Viewmodels
{
    public class ProdoctVM
    {
        public Product? Product { get; set; }
        public IEnumerable<SelectListItem>? CategorySelectList { get; set; }
        public IEnumerable<SelectListItem>? AplicationTypeSelectList { get; set; }

    }
}
