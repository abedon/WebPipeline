using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace WebApplication2
{
	public class MefControllerFactory : DefaultControllerFactory
	{
		private readonly CompositionContainer _container;
		private readonly Dictionary<IController, Lazy<object, object>> _exports;
		private readonly object _syncRoot;

		public MefControllerFactory(CompositionContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			_container = container;
			_exports = new Dictionary<IController, Lazy<object, object>>();
			_syncRoot = new object();
		}

		protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
		{
			var export = _container.GetExports(controllerType, null, null).SingleOrDefault();

			IController controller;
			if (null != export)
				controller = (IController)export.Value;
			else
			{
				controller = base.GetControllerInstance(requestContext, controllerType);
				_container.ComposeParts(controller);
			}

			lock (_syncRoot)
			{
				_exports.Add(controller, export);
			}

			return controller;
		}

		public override void ReleaseController(IController controller)
		{
			lock (_syncRoot)
			{
				var export = _exports[controller];
				_exports.Remove(controller);

				_container.ReleaseExport(export);
			}
			base.ReleaseController(controller);
		}
	}

	public class MefControllerFactory2 : IControllerFactory
	{
		private string _pluginPath;
		private DirectoryCatalog _catalog;
		private CompositionContainer _container;

		private DefaultControllerFactory defaultControllerFactory;

		public MefControllerFactory2(string pluginPath)
		{
			_pluginPath = pluginPath;
			_catalog = new DirectoryCatalog(pluginPath);
			_container = new CompositionContainer(_catalog);

			defaultControllerFactory = new DefaultControllerFactory();
		}

		public MefControllerFactory2(CompositionContainer compositionContainer)
		{
			_container = compositionContainer;
			defaultControllerFactory = new DefaultControllerFactory();
		}

		#region IControllerFactory Members

		public IController CreateController(RequestContext requestContext, string controllerName)
		{
			return defaultControllerFactory.CreateController(requestContext, controllerName);
		}

		public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
		{
			return System.Web.SessionState.SessionStateBehavior.Default;
		}

		public void ReleaseController(IController controller)
		{
			IDisposable disposable = controller as IDisposable;
			if (disposable != null)
				disposable.Dispose();
		}

		#endregion
	}
}