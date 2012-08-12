using System;
using System.Linq;
using AuthenticationExample.Web.Model;

namespace AuthenticationExample.Web.PersistenceSupport
{
	public interface IRepository
	{
		T Get<T>(Guid id) where T : Entity;
		IQueryable<T> GetAll<T>() where T : Entity;
		void SaveOrUpdate<T>(T entity) where T : Entity;
	}
}
