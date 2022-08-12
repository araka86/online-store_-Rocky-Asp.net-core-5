using Rocky_Model;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IOrderHeaderRepository : IRepository<OrderHeader>
    {
        void Update(OrderHeader obj);
    }
}
