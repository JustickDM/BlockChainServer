using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlockChainDataBase.Interfaces
{
	public interface IRepository<T> : IDisposable
		where T : class
	{
		Task<List<T>> GetEntityListAsync();
		Task<T> GetEntityAsync(int id);
		Task<bool> AddAsync(T item);
		Task<bool> AddRangeAsync(List<T> items);
		Task<bool> UpdateAsync(T item);
		Task<bool> DeleteAsync(int id);
	}
}
