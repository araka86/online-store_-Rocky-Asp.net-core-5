using Rocky_Model;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        void Update(OrderDetail obj);
    }
}
