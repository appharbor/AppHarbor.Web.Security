using System;

namespace AuthenticationExample.Web.Model
{
	public abstract class Entity
	{
		public virtual Guid Id { get; set; }
	}
}
