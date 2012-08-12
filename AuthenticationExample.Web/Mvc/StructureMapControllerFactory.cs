using System;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace AuthenticationExample.Web.Mvc
{
	public class StructureMapControllerFactory : DefaultControllerFactory
	{
		public override IController CreateController(RequestContext requestContext, string controllerName)
		{
			try
			{
				var controllerType = base.GetControllerType(requestContext, controllerName);
				return ObjectFactory.GetInstance(controllerType) as IController;
			}
			catch (Exception)
			{
				return base.CreateController(requestContext, controllerName);
			}
		}
	}
}
