using Rocky_Model;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky_DataAccess.Repository
{
    public class AplicationUserRepository : Repository<AplicationUser>, IAplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public AplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
