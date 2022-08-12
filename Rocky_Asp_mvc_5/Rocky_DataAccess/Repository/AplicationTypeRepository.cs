using Rocky_Model;
using Rocky_DataAccess.Repository.IReposotory;
namespace Rocky_DataAccess.Repository
{
    public class AplicationTypeRepository : Repository<AplicationType>, IAplicationTypeRepository
    {
        private readonly ApplicationDbContext _db;
        public AplicationTypeRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }
        public void Update(AplicationType obj)
        {
           var objFromDb = base.FirstOrDefault(u=>u.Id == obj.Id);
            if(objFromDb != null)
                objFromDb.Name = obj.Name;                       
        }  
    }
}
