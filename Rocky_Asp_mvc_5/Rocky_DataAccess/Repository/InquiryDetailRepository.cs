using Rocky_Model;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky_DataAccess.Repository
{
    public class InquiryDetailRepository : Repository<InquiryDetail>, IInquiryDetailRepository
    {
        private readonly ApplicationDbContext _db;
        public InquiryDetailRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(InquiryDetail obj)
        {
            _db.InquiryDetail.Update(obj);
        }       
    }
}
