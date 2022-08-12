using System.Linq.Expressions;
namespace Rocky_DataAccess.Repository.IReposotory
{
    public interface IRepository<T> where T : class
    {
        T Find(int id);
        IEnumerable<T> GetAll(
                            Expression<Func<T, bool>>? filter = null, //Видиления неких обьектов
                            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, //порядок вывода перечисляемых функций
                            string? includeProperties = null, //для свойств
                            bool isTracking = true
                            );

        T FirstOrDefault(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null,
            bool isTracking = true
            );
        void Add(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entity);
        void Save();
    }
}
