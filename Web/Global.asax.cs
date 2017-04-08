using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using System.Linq;
using WebApplication2.Providers;
using Utils;

namespace WebApplication2
{
	public class WebApiApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			MefConfig.Register("Version1");
			AreaRegistration.RegisterAllAreas();
			GlobalConfiguration.Configure(WebApiConfig.Register);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);

            //set up channels (process orders)
            try
            {
                SetUpChannels();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Please make sure the configuration file specifies the correct channel names and mappings. " + ex.Message);
            }
        }

        //We load all channels into memory as a dictionary, function module can look up this dictionary by request type to get the imports(dependencies).
        private static void SetUpChannels()
        {
            try
            {
                var channelMappings = new Dictionary<string, List<string>>();
                var strChannelMappings = Properties.Settings.Default.channelMapping.Cast<string>().ToList();
                foreach (var strChannelMapping in strChannelMappings)
                {
                    var arrChannelMapping = strChannelMapping.Split(":".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    var controllerName = arrChannelMapping[0].Trim().ToLower();
                    var channelname = arrChannelMapping[1].Trim().ToLower();

                    var channelContent = ConfigurationManager.AppSettings[channelname].Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    //remove spaces
                    for (var i = 0; i < channelContent.Length; i++)
                    {
                        channelContent[i] = channelContent[i].Trim().ToLower();
                    }
                    channelMappings.Add(controllerName, channelContent.ToList());
                }

                ProcessingFlow.SetUp(channelMappings);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
