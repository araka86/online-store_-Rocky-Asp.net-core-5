namespace Rocky_Model.Viewmodels
{
    public class DetailsVM
    {
        public DetailsVM()
        {
            Product = new Product();
        }

        public Product Product { get; set; }

        public bool ExistInCart { get; set; }


    }
}
