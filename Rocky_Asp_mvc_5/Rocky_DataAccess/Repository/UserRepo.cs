using Microsoft.EntityFrameworkCore;
using Rocky_Model;
using Microsoft.AspNetCore.Mvc;
namespace Rocky_DataAccess.Repository
{
    public class UserRepo : Controller
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<AplicationUser> dbSet;
        public UserRepo(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<AplicationUser>();
        }
        public IQueryable<AplicationUser> FirstOrDefault(string obj)
        {
            IQueryable <AplicationUser> query = dbSet;
            var data = query.Where(x => x.Id == obj);
            return data;
        }
    }
}
