using SportsCenterApi.Repositories;

namespace SportsCenterApi.Services
{
    public class GenericService<T> : IGenericService<T> where T : class
    {

        protected readonly IGenericRepository<T> _repository;
        public GenericService(IGenericRepository<T> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            return await _repository.AddAsync(entity);
        }

        public virtual async Task<T?> UpdateAsync(int id, T entity)
        {
            var existingEntity = await _repository.GetByIdAsync(id);
            if (existingEntity == null) return null;

            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
