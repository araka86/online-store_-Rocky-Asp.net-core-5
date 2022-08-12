namespace Rocky_Model.Viewmodels
{
    public class HomeVm
    {
        public IEnumerable<Product>? Products { get; set; }
        public IEnumerable<Category>? Categories { get; set; }

        public IEnumerable<AplicationType>? AplicationTypes { get; set; }
    }
}
