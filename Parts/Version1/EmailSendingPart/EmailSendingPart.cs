/* 
 * WebPipeline 
 * https://github.com/abedon/WebPipeline 
 * 
 * Copyright (c) 2017 Dong Chen
 * Licensed under the Apache 2.0 License. 
 */

using Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Parts
{
	public class EmailSendingPart : BasePart, ICompositionPart
    {
        public new IGlobalOrderRepository Process(IGlobalOrderRepository globalOrderRepository, string controllerName)
        {
            var orders = globalOrderRepository.GetOrdersByBiller(1);

            foreach (var order in orders)
            {
                order.Biller.Name = order.Biller.Name + " (" + this.GetType().ToString() + "1)";
            }

            return Next(globalOrderRepository, this.GetType().ToString(), controllerName);
        }
    }
}
