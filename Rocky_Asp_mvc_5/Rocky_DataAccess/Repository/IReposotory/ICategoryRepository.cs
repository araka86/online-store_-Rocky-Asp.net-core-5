using Rocky_Model;

namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface ICategoryRepository: IRepository<Category>
    {
        void Update(Category obj);
    }
}
