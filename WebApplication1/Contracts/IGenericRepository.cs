using ClassLibrary1.Models;

namespace WebApplication1.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id); // 用於根據 ID 獲取單一實體
        Task<TResult> GetAsync<TResult>(int? id);
        Task<List<T>> GetAllAsync(); // 用於獲取所有實體的列表
        Task<List<TResult>> GetAllAsync<TResult>();
        Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
        Task<T> AddAsync(T entity); // 用於添加新實體
        Task<TResult> AddAsync<TSource, TResult>(TSource source);
        Task DeleteAsync(int id); // 用於根據 ID 刪除實體
        Task UpdateAsync(T entity); // 用於更新實體
        Task UpdateAsync<TSource>(int id, TSource source);
        Task<bool> Exists(int id); // 用於檢查實體是否存在
    }
}
