using Rocky_Model;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky_DataAccess.Repository
{
    public class InquiryHeaderRepository : Repository<InquiryHeader>, IInquiryHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public InquiryHeaderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(InquiryHeader obj)
        {
            _db.InquiryHeader.Update(obj);
        }
        
    }
}
