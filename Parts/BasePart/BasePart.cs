using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Utils;

namespace Parts
{
	public abstract class BasePart
	{
        [ImportMany(AllowRecomposition = true)]
        protected IEnumerable<ICompositionPart> _compositionParts = null;

        protected ICompositionPart _nextPart;

		public IGlobalOrderRepository Process(IGlobalOrderRepository globalOrderRepository, string controllerName)
		{
            var orders = globalOrderRepository.GetOrdersByBiller(1);

            foreach (var order in orders)
            {
                order.Biller.Name = order.Biller.Name + " (" + this.GetType().ToString() + ")";
            }

            return Next(globalOrderRepository, this.GetType().ToString(), controllerName);
        }

        public IGlobalOrderRepository Next(IGlobalOrderRepository globalOrderRepository, string currentPartName, string controllerName)
        {
            var nextPartName = ProcessingFlow.GetNextPartName(currentPartName, controllerName);
            if (nextPartName == Constants.BuiltInPart.FINALIZING)
                return globalOrderRepository;

            _nextPart = ProcessingFlow.GetPart(nextPartName, _compositionParts);
            if (_nextPart != null)
            {
                return _nextPart.Process(globalOrderRepository, controllerName);
            }
            else
            {
                return Next(globalOrderRepository, nextPartName, controllerName);
            }
        }        
    }
}
