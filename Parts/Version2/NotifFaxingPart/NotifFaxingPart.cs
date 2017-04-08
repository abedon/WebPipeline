using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Parts
{
	public class NotifFaxingPart : BasePart, ICompositionPart
    {
        public new IGlobalOrderRepository Process(IGlobalOrderRepository globalOrderRepository, string controllerName)
        {
            var orders = globalOrderRepository.GetOrdersByBiller(1);

            foreach (var order in orders)
            {
                order.Biller.Name = order.Biller.Name + " (" + this.GetType().ToString() + "2)";
            }

            return Next(globalOrderRepository, this.GetType().ToString(), controllerName);
        }
    }
}
