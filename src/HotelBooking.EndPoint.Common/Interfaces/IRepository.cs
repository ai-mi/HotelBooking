using HotelBooking.EndPoint.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.EndPoint.Common.Interfaces
{
	public interface IRepository<T> where T : Base
	{
		Task<T?> GetByIdAsync(Guid id);
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		Task<T> AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(T entity);
		Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
	}
}
