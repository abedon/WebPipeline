using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Dependencies;
using System.Web.Mvc;

namespace WebApplication2
{
	public class MefConfig
	{
		public static void Register(string partsVersion)
		{
			var registrationBuilder = new RegistrationBuilder();

			registrationBuilder
				.ForTypesDerivedFrom<Controller>()
				.SetCreationPolicy(CreationPolicy.NonShared)
				.Export();

			registrationBuilder
				.ForTypesDerivedFrom<ApiController>()
				.SetCreationPolicy(CreationPolicy.NonShared)
				.Export();

			registrationBuilder
				.ForTypesMatching(t => IsDescendentOf(t, typeof(IGlobalOrderRepository)))
				.Export(builder => builder.AsContractType(typeof(IGlobalOrderRepository)))
				.SetCreationPolicy(CreationPolicy.NonShared);

			registrationBuilder
				.ForTypesMatching(t => IsDescendentOf(t, typeof(ICompositionPart)))
				.Export(builder => builder.AsContractType(typeof(ICompositionPart)))
				.SetCreationPolicy(CreationPolicy.Shared);

			var aggregateCatalog = new AggregateCatalog();

			aggregateCatalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly(), registrationBuilder));

			var path = HostingEnvironment.MapPath("~/bin");
			// Custom controllers
			aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path, "*Parts*.dll", registrationBuilder));

			// Global object flowing through the pipeline
			aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path, "*Repository*.dll", registrationBuilder));

			// The version needs to be updated somewhere permanently (File, DB, etc.)
			// Otherwise every time app is restarted (e.g. triggered by dll change), version will be reset.
			path = HostingEnvironment.MapPath("~/Parts/" + partsVersion);
			// Functional modules
			aggregateCatalog.Catalogs.Add(new DirectoryCatalog(path, "*Part.*dll", registrationBuilder));

			var container = new CompositionContainer(aggregateCatalog);
			container.ComposeParts();

			// Apply MEF to MVC and WebAPI

			// Option1: Install MEF dependency resolver
			var resolver = new MefDependencyResolver(container);

			// for Web API
			GlobalConfiguration.Configuration.DependencyResolver = resolver;

			// for MVC (Note: This does NOT require SetControllerFactory)
			DependencyResolver.SetResolver(resolver);

			// Option 2: Set MEF controller factory
			// Note: This does NOT require SetResolver for MVC
			//ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory(container));

			// An alternative (Note: This requires SetResolver for MVC)
			//ControllerBuilder.Current.SetControllerFactory(new MefControllerFactory2(container));
		}
        
        private static bool IsGenericDescendentOf(TypeInfo openType, TypeInfo baseType)
		{
			if (openType.BaseType == null)
			{
				return false;
			}
			if (openType.BaseType == baseType.AsType())
			{
				return true;
			}
			foreach (Type type in openType.ImplementedInterfaces)
			{
				if (type.IsConstructedGenericType && (type.GetGenericTypeDefinition() == baseType.AsType()))
				{
					return true;
				}
			}
			return IsGenericDescendentOf(IntrospectionExtensions.GetTypeInfo(openType.BaseType), baseType);
		}

		private static bool IsDescendentOf(Type type, Type baseType)
		{
			if (((type == baseType) || (type == typeof(object))) || (type == null))
			{
				return false;
			}
			TypeInfo typeInfo = IntrospectionExtensions.GetTypeInfo(type);
			TypeInfo info2 = IntrospectionExtensions.GetTypeInfo(baseType);
			if (typeInfo.IsGenericTypeDefinition)
			{
				return IsGenericDescendentOf(typeInfo, info2);
			}
			return info2.IsAssignableFrom(typeInfo);
		}
	}	
}