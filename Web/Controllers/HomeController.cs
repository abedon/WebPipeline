using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
	public class HomeController : Controller
	{
        private IGlobalOrderRepository _globalOrderRepository = null;

        public HomeController(IGlobalOrderRepository globalOrderRepository)
        {
            _globalOrderRepository = globalOrderRepository;
        }

        public ActionResult Index()
		{
			var contacts = _globalOrderRepository.GetNewPAPSwitchOrders();
			return View();
		}

        public String UseVer(int? version)
        {
            if (version == null)
                version = 1;

            MefConfig.Register("Version" + version);
            return "Using Version" + version;
        }        
    }
}
