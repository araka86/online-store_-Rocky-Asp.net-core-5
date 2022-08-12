using System.Collections.ObjectModel;
namespace Rocky_Utility
{
    public static class WebConstanta
    {
        //For Path to image
        public const string ImagePath = @"\images\product\";
        //For session
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";
        //For roles
        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";
        //For mail
        public const string EmailAdmin = "araka86@gmail.com";
        public const string CategoryName = "Category";
        public const string AplicationTypeName = "AplicationType";
        //For porcess message
        public const string Success = "Success";
        public const string Error = "Error";
        //for status order
        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusProcessing = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";
        public static readonly IEnumerable<string> listStatus = new ReadOnlyCollection<string>(
           new List<string>
           {
               StatusApproved,
               StatusPending,
               StatusShipped,
               StatusCancelled,
               StatusRefunded
           });
    }

    public enum ListStatus1
    {
        Approved,
        Pending,
        Shipped,
        Cancelled,
        Refunded
    }
}
