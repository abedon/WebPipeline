using System.Linq;
using System.Web;
using System.IO;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Reflection;

namespace PluginFramework.Plugins
{
	public class PreApplicationInit
	{

		static PreApplicationInit()
		{
			PluginFolder = new DirectoryInfo(HostingEnvironment.MapPath("~/ControllerParts"));
			ShadowCopyFolder = new DirectoryInfo(HostingEnvironment.MapPath("~/ControllerParts/temp"));
		}

		/// <summary>
		/// The source plugin folder from which to shadow copy from
		/// </summary>
		/// <remarks>
		/// This folder can contain sub folderst to organize plugin types
		/// </remarks>
		private static readonly DirectoryInfo PluginFolder;

		/// <summary>
		/// The folder to shadow copy the plugin DLLs to use for running the app
		/// </summary>
		private static readonly DirectoryInfo ShadowCopyFolder;

		public static void Initialize()
		{
			Directory.CreateDirectory(ShadowCopyFolder.FullName);

			//clear out plugins)
			foreach (var f in ShadowCopyFolder.GetFiles("*.dll", SearchOption.AllDirectories))
			{
				f.Delete();
			}

			//shadow copy files
			foreach (var plug in PluginFolder.GetFiles("*.dll", SearchOption.AllDirectories))
			{
				var di = Directory.CreateDirectory(Path.Combine(ShadowCopyFolder.FullName, plug.Directory.Name));
				// NOTE: You cannot rename the plugin DLL to a different name, it will fail because the assembly name is part if it's manifest
				// (a reference to how assemblies are loaded: http://msdn.microsoft.com/en-us/library/yx7xezcf )
				File.Copy(plug.FullName, Path.Combine(di.FullName, plug.Name), true);
			}

			// Now, we need to tell the BuildManager that our plugin DLLs exists and to reference them.
			// There are different Assembly Load Contexts that we need to take into account which 
			// are defined in this article here:
			// http://blogs.msdn.com/b/suzcook/archive/2003/05/29/57143.aspx

			// * This will put the plugin assemblies in the 'Load' context
			// This works but requires a 'probing' folder be defined in the web.config
			foreach (var a in
				ShadowCopyFolder
				.GetFiles("*.dll", SearchOption.AllDirectories)
				.Select(x => AssemblyName.GetAssemblyName(x.FullName))
				.Select(x => Assembly.Load(x.FullName)))
			{
				BuildManager.AddReferencedAssembly(a);
			}

			// * This will put the plugin assemblies in the 'LoadFrom' context
			// This works but requires a 'probing' folder be defined in the web.config
			// This is the slowest and most error prone version of the Load contexts.            
			//foreach (var a in
			//    ShadowCopyFolder
			//    .GetFiles("*.dll", SearchOption.AllDirectories)
			//    .Select(plug => Assembly.LoadFrom(plug.FullName)))
			//{
			//    BuildManager.AddReferencedAssembly(a);
			//}

			// * This will put the plugin assemblies in the 'Neither' context ( i think )
			// This nearly works but fails during view compilation.
			// This DOES work for resolving controllers but during view compilation which is done with the RazorViewEngine, 
			// the CodeDom building doesn't reference the plugin assemblies directly.
			//foreach (var a in
			//    ShadowCopyFolder
			//    .GetFiles("*.dll", SearchOption.AllDirectories)
			//    .Select(plug => Assembly.Load(File.ReadAllBytes(plug.FullName))))
			//{
			//    BuildManager.AddReferencedAssembly(a);
			//}

		}
	}
}
