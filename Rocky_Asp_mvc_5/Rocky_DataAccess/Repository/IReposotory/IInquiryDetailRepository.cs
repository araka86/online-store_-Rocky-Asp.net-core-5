using Rocky_Model;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IInquiryDetailRepository : IRepository<InquiryDetail>
    {
        void Update(InquiryDetail obj);
    }
}
