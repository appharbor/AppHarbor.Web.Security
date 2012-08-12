using System;
using System.Collections.Generic;
using System.Linq;
using AuthenticationExample.Web.Model;
using AuthenticationExample.Web.PersistenceSupport;

namespace AuthenticationExample.Web
{
	public class InMemoryRepository : IRepository
	{
		private static IDictionary<Type, IDictionary<Guid, object>> _dictionaries =
			new Dictionary<Type, IDictionary<Guid, object>>();

		public T Get<T>(Guid id) where T : Entity
		{
			IDictionary<Guid, object> dictionary;
			if (_dictionaries.TryGetValue(typeof(T), out dictionary))
			{
				return (T)dictionary[id];
			}

			return default(T);
		}

		public void SaveOrUpdate<T>(T entity) where T : Entity
		{
			var type = typeof(T);
			IDictionary<Guid, object> dictionary;
			if (!_dictionaries.TryGetValue(type, out dictionary))
			{
				dictionary = new Dictionary<Guid, object>();
				_dictionaries.Add(type, dictionary);
			}

			dictionary[entity.Id] = entity;
		}

		public IQueryable<T> GetAll<T>() where T : Entity
		{
			var type = typeof(T);
			if (_dictionaries.ContainsKey(type))
			{
				return _dictionaries[type].Values.OfType<T>().AsQueryable();
			}

			return Enumerable.Empty<T>().AsQueryable();
		}
	}
}
