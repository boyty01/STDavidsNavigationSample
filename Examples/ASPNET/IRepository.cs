

namespace StDavidsQRNavigation.Services
{
    public interface IRepository<T> where T : class
    {
        Task<ServiceResult<T>> Create(T model);
        Task<ServiceResult<T>> Get(int id, bool includeRelated = false);
        Task<ServiceResult<List<T>>> GetAll(bool includeRelated = false);
        Task<ServiceResult<T>> Update(T model);
        Task<ServiceResult<bool>> Delete(int id);
    }
}
