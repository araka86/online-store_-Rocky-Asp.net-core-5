using Rocky_Model;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader obj);
    }
}
