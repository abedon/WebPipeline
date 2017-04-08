using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Web.Mvc;
using Utils;

namespace ControllerParts
{
	public class CustomController : Controller
    {
        private IGlobalOrderRepository _globalOrderRepository = null;

        [ImportMany(AllowRecomposition = true)]
		private IEnumerable<ICompositionPart> _compositionParts = null;

		public CustomController(IGlobalOrderRepository globalOrderRepository)
		{
            _globalOrderRepository = globalOrderRepository;
        }

		public ActionResult Index()
		{
            var globalOrderRepository = Process(_globalOrderRepository);

            var orders = globalOrderRepository.GetOrdersByBiller(1);

            string billerNames = "";
            foreach (var order in orders)
            {
                billerNames += order.Biller.Name + "\n";
            }

			return View(orders);
		}        

        public IGlobalOrderRepository Process(IGlobalOrderRepository globalOrderRepository)
		{
            var orders = globalOrderRepository.GetOrdersByBiller(1);

            foreach (var order in orders)
            {
                order.Biller.Name = order.Biller.Name + " (Init)";
            }

            return Next(globalOrderRepository, null);
		}

        public IGlobalOrderRepository Next(IGlobalOrderRepository globalOrderRepository, string currentPartName)
        {
            if (string.IsNullOrWhiteSpace(currentPartName))
                currentPartName = Constants.BuiltInPart.INITIALIZING;

            var nextPartName = ProcessingFlow.GetNextPartName(currentPartName, this.GetType().ToString());
            if (nextPartName == Constants.BuiltInPart.FINALIZING)
                return globalOrderRepository;

            var nextPart = ProcessingFlow.GetPart(nextPartName, _compositionParts);            
            if (nextPart != null)
            {
                return nextPart.Process(globalOrderRepository, this.GetType().ToString());
            }
            else
                return Next(globalOrderRepository, nextPartName);
        }
	}


}
