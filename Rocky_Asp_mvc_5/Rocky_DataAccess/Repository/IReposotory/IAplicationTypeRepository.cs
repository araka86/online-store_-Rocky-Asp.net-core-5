using Rocky_Model;

namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IAplicationTypeRepository : IRepository<AplicationType>
    {
        void Update(AplicationType obj);
    }
}
